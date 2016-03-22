using System.Configuration;
using System.Data.SqlClient;

class DoSomethingToAccessDataFromMSSQL : ISimulateUserDataAccess
{
    public void DoSomething()
    {
        using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"].ConnectionString))
        using (var command = db.CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) From Subscription;";

            db.Open();
            command.ExecuteScalar();
        }
    }
}
