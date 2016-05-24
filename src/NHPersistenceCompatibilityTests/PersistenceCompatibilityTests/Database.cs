namespace PersistenceCompatibilityTests
{
    using System.Data.SqlClient;
    using Common;

    public class Database
    {
        public static void Cleanup()
        {
            var connectionString = NHibernateConnectionInfo.ConnectionString;
            var builder = new SqlConnectionStringBuilder(connectionString);
            var initialCatalog = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                var query = @"IF EXISTS(SELECT * from sys.databases where name = '{0}')
                              BEGIN 
                                ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
                                DROP DATABASE [{0}]
                              END 
                            CREATE DATABASE [{0}]";

                command.CommandText = string.Format(query, initialCatalog);
                command.ExecuteNonQuery();
            }
        }
    }
}