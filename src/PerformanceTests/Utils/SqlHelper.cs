using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

public static class SqlHelper
{
    public static void ExecuteScript(string connectionString, string sql)
    {
        var commands = Regex
            .Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Where(commandString => !string.IsNullOrWhiteSpace(commandString));

        using (var con = new SqlConnection(connectionString))
        {
            con.Open();
            foreach (var cmdText in commands)
            {
                using (var command = new SqlCommand(cmdText, con))
                {
                    command.ExecuteNonQuery();
                }
            }
            con.Close();
        }
    }
}