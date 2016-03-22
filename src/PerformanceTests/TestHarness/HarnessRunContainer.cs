using NServiceBus;
using NServiceBus.Persistence;
using Raven.Client.Document;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Messaging;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.RavenDB.Outbox;
using Raven.Abstractions.Indexing;
using Raven.Json.Linq;

public class HarnessRunContainer : IHandleCompleted, IDisposable
{
    public const int ThroughputPerSecond = 20;

    TaskCompletionSource<long> processingCompletionSource;
    Stopwatch trackingStopwatch;

    IStartableBus incomingBus;
    IStartableBus senderBus;

    ExactOnce exactOnce;
    Persistence persistence;
    int batchesToSend;

    public HarnessRunContainer(ExactOnce exactOnce, Persistence persistence, int batchesToSend = 1)
    {
        this.exactOnce = exactOnce;
        this.persistence = persistence;
        this.batchesToSend = batchesToSend;

        ResetEnvironment();
        CreateBusses();

        if (exactOnce == ExactOnce.Outbox)
        {
            Seed(persistence);
        }
    }

    public void Seed(Persistence persistence)
    {
        if (persistence == Persistence.RavenDB)
            PopulatePreviousOutboxRecordsRaven();
        else
            PopulatePreviousOutboxRecordsSql();
    }

    public TimeSpan Run()
    {
        SeedMessagesToProcess(batchesToSend);

        var processTask = ProcessIncomingMessages();
        processTask.Wait();

        return TimeSpan.FromMilliseconds(processTask.Result);
    }

    void ResetEnvironment()
    {
        if (persistence == Persistence.RavenDB)
        {
            PurgeOutboxRecordsForRaven();
        }
        else
        {
            PurgeOutboxRecords();
        }

        PurgeQueues();
    }

    void PurgeQueues()
    {
        var privateQueues = MessageQueue.GetPrivateQueuesByMachine("localhost");
        foreach (var queue in privateQueues)
        {
            queue.Purge();
        }
    }

    void PopulatePreviousOutboxRecordsRaven()
    {
        var period = TimeSpan.Parse(ConfigurationManager.AppSettings["NServiceBus/Outbox/NHibernate/TimeToKeepDeduplicationData"]);
        var endTime = DateTime.UtcNow;
        var numberOfMesssagesBetween = period.TotalSeconds*ThroughputPerSecond;
        var startTime = endTime - period;
        Console.WriteLine("RavenDB: outbox records seed. Records: {0:N0}, From: {1:s}, To: {2:s}, Throughput: ~{3:N0}/s", numberOfMesssagesBetween, startTime, endTime, numberOfMesssagesBetween/period.TotalSeconds);
        var millisecondsBetweenMessages = (endTime - startTime).TotalMilliseconds/numberOfMesssagesBetween;

        using (var store = new DocumentStore
        {
            ConnectionStringName = "NServiceBus/Persistence/RavenDB",
            DefaultDatabase = "TestHarness.IncomingMessages"
        })
        {
            store.Conventions.FindTypeTagName = t => t.Name;
            store.Initialize();

            using (var bulk = store.BulkInsert())
            {
                for (var msgDate = startTime;
                    msgDate < endTime;
                    msgDate += TimeSpan.FromMilliseconds(millisecondsBetweenMessages))
                {
                    var id = Guid.NewGuid().ToString();
                    bulk.Store(
                        new RavenJObject
                        {
                            {"MessageId", id},
                            {"Dispatched", true},
                            {"DispatchedAt", msgDate},
                            {"TransportOperations", RavenJToken.FromObject(new string[]
                            {
                            })
                            }
                        },
                        new RavenJObject
                        {
                            {"Raven-Entity-Name", "OutboxRecord"},
                            {"Raven-Clr-Type", "NServiceBus.RavenDB.Outbox.OutboxRecord, NServiceBus.RavenDB"}
                        },
                        "Outbox/TestHarness.IncomingMessages/" + id
                        );
                }


                while (store.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
                {
                    System.Threading.Thread.Sleep(10);
                    Console.Write(".");
                }

                if (null == store.DatabaseCommands.GetIndex("OutboxRecordsIndex"))
                {
                    Console.WriteLine("Creating index: OutboxRecordsIndex");
                    store.DatabaseCommands.PutIndex("OutboxRecordsIndex", new IndexDefinition
                    {
                        Map = @"from doc in docs.OutboxRecord select new { MessageId = doc.MessageId, Dispatched = doc.Dispatched, DispatchedAt = doc.DispatchedAt }"
                    });
                }

                while (store.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
                {
                    System.Threading.Thread.Sleep(10);
                    Console.Write(".");
                }

            }

            var duration = DateTime.UtcNow - endTime;
            Console.WriteLine("Outbox records inserted in {0:g} ({1:N0}/s)", duration, numberOfMesssagesBetween/duration.TotalSeconds);
        }
    }

    void PopulatePreviousOutboxRecordsSql()
    {
        var period = TimeSpan.Parse(ConfigurationManager.AppSettings["NServiceBus/Outbox/NHibernate/TimeToKeepDeduplicationData"]);
        var endTime = DateTime.UtcNow;
        var startTime = endTime - period;
        var numberOfMesssagesBetween = period.TotalSeconds*ThroughputPerSecond;
        Console.WriteLine("SQL: outbox records seed. Records: {0:N0}, From: {1:s}, To: {2:s}, Throughput: ~{3:N0}/s", numberOfMesssagesBetween, startTime, endTime, numberOfMesssagesBetween/period.TotalSeconds);
        var millisecondsBetweenMessages = (endTime - startTime).TotalMilliseconds/numberOfMesssagesBetween;


        using (var dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"].ConnectionString))
        {
            dbConnection.Open();
            using (var tx = dbConnection.BeginTransaction())
            {
                const int batchSize = 15;
                var i = 0;
                var command = dbConnection.CreateCommand();
                command.Transaction = tx;
                var sql = new StringBuilder(batchSize*128);

                for (var msgDate = startTime; msgDate < endTime; msgDate += TimeSpan.FromMilliseconds(millisecondsBetweenMessages))
                {
                    i++;
                    sql.AppendFormat("INSERT INTO OutboxRecord(MessageId,Dispatched,DispatchedAt)VALUES(@id{0},1,@ts{0});", i);
                    command.Parameters.AddWithValue("id" + i, "TestHarness.IncomingMessages/" + Guid.NewGuid());
                    command.Parameters.AddWithValue("ts" + i, msgDate);


                    if (i != batchSize) continue;

                    command.CommandText = sql.ToString();
                    command.ExecuteScalar();
                    command.Dispose();
                    command = dbConnection.CreateCommand();
                    command.Transaction = tx;
                    sql.Clear();
                    i = 0;
                }

                if (i > 0)
                {
                    command.CommandText = sql.ToString();
                    command.ExecuteScalar();
                    command.Dispose();
                }
                tx.Commit();
            }
        }
        var duration = DateTime.UtcNow - endTime;
        Console.WriteLine("Outbox records inserted in {0:g} ({1:N0}/s)", duration, numberOfMesssagesBetween/duration.TotalSeconds);
    }



    void PurgeOutboxRecords()
    {
        using (var dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"].ConnectionString))
        {
            try
            {
                dbConnection.Open();
            }
            catch (SqlException)
            {
                CreateDatabase(dbConnection);
                return;
            }

            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = @"IF OBJECT_ID('OutboxRecord') IS NOT NULL    
BEGIN
TRUNCATE TABLE OutboxRecord;
TRUNCATE TABLE Subscription;
TRUNCATE TABLE TimeoutEntity;
END";

                command.ExecuteNonQuery();
            }
        }
    }

    static void CreateDatabase(SqlConnection dbConnection)
    {
        var builder = new SqlConnectionStringBuilder(dbConnection.ConnectionString);
        var originalDb = builder.InitialCatalog;
        builder.InitialCatalog = "master";
        using (var masterDb = new SqlConnection(builder.ConnectionString))
        {
            masterDb.Open();

            using (var createDbCommand = masterDb.CreateCommand())
            {
                createDbCommand.CommandText = "CREATE DATABASE " + originalDb;
                createDbCommand.ExecuteNonQuery();
            }
        }
    }

    void PurgeOutboxRecordsForRaven()
    {
        using (var store = new DocumentStore
        {
            ConnectionStringName = "NServiceBus/Persistence/RavenDB"
        })
        {
            store.Initialize(); // initializes document store, by connecting to server and downloading various configurations

            var headers = store.DatabaseCommands.Head("Raven/Databases/TestHarness.IncomingMessages");
            if (headers != null)
            {
                store.DatabaseCommands.GlobalAdmin.DeleteDatabase("TestHarness.IncomingMessages", true);
            }
            store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("TestHarness.IncomingMessages");
        }
    }

    void CreateBusses()
    {
        if (exactOnce == ExactOnce.DTC)
        {
            EnsureDtcRunning();
        }
        else
        {
            EnsureDtcStopped();
        }

        CreateSenderBus();
        CreateIncomingBus();
        CreateTradeBusses();
        CreateNotificationBus();
    }

    void EnsureDtcStopped()
    {
        using (var dtcService = new ServiceController("Distributed Transaction Coordinator"))
        {
            if (dtcService.Status != ServiceControllerStatus.Stopped)
            {
                dtcService.Stop();
                dtcService.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            }
        }
    }

    void EnsureDtcRunning()
    {
        using (var dtcService = new ServiceController("Distributed Transaction Coordinator"))
        {
            if (dtcService.Status != ServiceControllerStatus.Running)
            {
                dtcService.Start();
                dtcService.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
            }
        }
    }

    void CreateSenderBus()
    {
        var busConfiguration = CreateBusConfiguration("TestHarness.Sender");

        senderBus = Bus.Create(busConfiguration);
    }

    void CreateTradeBusses()
    {
        var busConfiguration = CreateBusConfiguration("TestHarness.Shorts");

        Bus.Create(busConfiguration);

        busConfiguration = CreateBusConfiguration("TestHarness.Longs");

        Bus.Create(busConfiguration);
    }

    void CreateNotificationBus()
    {
        var busConfiguration = CreateBusConfiguration("TestHarness.Notifications");

        Bus.Create(busConfiguration);
    }

    BusConfiguration CreateBusConfiguration(string endpointName)
    {
        var busConfiguration = new BusConfiguration();

        if (exactOnce == ExactOnce.Outbox)
        {
            busConfiguration.EnableOutbox();
            busConfiguration.SetTimeToKeepDeduplicationData(TimeSpan.Parse(ConfigurationManager.AppSettings["NServiceBus/Outbox/NHibernate/TimeToKeepDeduplicationData"]));
            busConfiguration.SetFrequencyToRunDeduplicationDataCleanup(TimeSpan.Parse(ConfigurationManager.AppSettings["NServiceBus/Outbox/NHibernate/FrequencyToRunDeduplicationDataCleanup"]));
        }

        if (persistence == Persistence.RavenDB)
        {
            busConfiguration.UsePersistence<RavenDBPersistence>();
            busConfiguration.RegisterComponents(x => x.ConfigureComponent(typeof(DoSomethingToAccessDataFromRavenDb), DependencyLifecycle.InstancePerCall));
        }
        else
        {
            busConfiguration.UsePersistence<NHibernatePersistence>();
            busConfiguration.RegisterComponents(x => x.ConfigureComponent(typeof(DoSomethingToAccessDataFromMSSQL), DependencyLifecycle.InstancePerCall));
        }

        busConfiguration.EnableInstallers();
        busConfiguration.EndpointName(endpointName);

        return busConfiguration;
    }

    void CreateIncomingBus()
    {
        var busConfiguration = CreateBusConfiguration("TestHarness.IncomingMessages");

        incomingBus = Bus.Create(busConfiguration);
    }

    void SeedMessagesToProcess(int batchSize)
    {
        var start = Stopwatch.StartNew();
        for (var x = 0; x < batchSize; x++)
        {
            var messages = MessageGenerator.GenerateMessages(650, 50, 100, 5);

            Parallel.ForEach(messages, new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount*4
            }, msg =>
            {
                MessageCounter.Increment();
                senderBus.Send(msg);
            });

            Console.WriteLine("Batch of 805 messages added to queue. Total current messages: {0:N0}", MessageCounter.Current());
        }

        var duration = start.ElapsedMilliseconds;
        Console.WriteLine("Message seed duration: {0:N0}ms, {1:N0} messages, {2:N0} msg/s", duration, MessageCounter.Current(), MessageCounter.Current()*1000/duration);
    }

    Task<long> ProcessIncomingMessages()
    {
        processingCompletionSource = new TaskCompletionSource<long>();

        MessageCounter.SubscribeToCompleted(this);

        trackingStopwatch = Stopwatch.StartNew();
        incomingBus.Start();

        return processingCompletionSource.Task;
    }

    public void Completed()
    {
        processingCompletionSource?.SetResult(trackingStopwatch.ElapsedMilliseconds);
    }

    public void Dispose()
    {
        // Must be empty
    }
}