using System.Configuration;
using System.Data.SqlClient;

namespace ServiceControlCompatibilityTests
{
    class SqlTransportDetails : ITransportDetails
    {
        const string TransportTypeName = "NServiceBus.SqlServerTransport, NServiceBus.Transports.SQLServer";

        public SqlTransportDetails(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string TransportName => "SQLServer";

        public void ApplyTo(Configuration configuration)
        {
            configuration.ConnectionStrings.ConnectionStrings.Set("NServiceBus/Transport", connectionString);
            var settings = configuration.AppSettings.Settings;
            settings.Set(SettingsList.TransportType, TransportTypeName);
        }

        public void EnsurePrerequisites()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"IF NOT  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Particular.ServiceControl]') AND type in (N'U'))
                  BEGIN
                    CREATE TABLE [dbo].[Particular.ServiceControl](
	                    [Id] [uniqueidentifier] NOT NULL,
	                    [CorrelationId] [varchar](255) NULL,
	                    [ReplyToAddress] [varchar](255) NULL,
	                    [Recoverable] [bit] NOT NULL,
	                    [Expires] [datetime] NULL,
	                    [Headers] [varchar](max) NOT NULL,
	                    [Body] [varbinary](max) NULL,
	                    [RowVersion] [bigint] IDENTITY(1,1) NOT NULL
                    ) ON [PRIMARY];                    

                    CREATE CLUSTERED INDEX [Index_RowVersion] ON [dbo].[Particular.ServiceControl] 
                    (
	                    [RowVersion] ASC
                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                    
                    CREATE NONCLUSTERED INDEX [Index_Expires] ON [dbo].[Particular.ServiceControl]
                    (
	                    [Expires] ASC
                    )
                    INCLUDE
                    (
                        [Id],
                        [RowVersion]
                    )
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

                  END";

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        string connectionString;
    }
}