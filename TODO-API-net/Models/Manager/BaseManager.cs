//using MySql.Data.MySqlClient;
//using System;

//namespace konnect_player_info.Model.Manager
//{
//    public class BaseManager
//    {
//        public static string ConnectionString { get; set; }

//        public MySqlConnection GetConnection(string dbName = "Andrei")
//        {
//            var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST");
//            var DB_PORT = Environment.GetEnvironmentVariable("DB_PORT");
//            var DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");
//            var DB_USER = Environment.GetEnvironmentVariable("DB_USER");
//            var DB_PASS = Environment.GetEnvironmentVariable("_DB_PASS");

//            var conn = $"Server={DB_HOST};Port={DB_PORT};Database={DB_NAME};Uid={DB_USER};Pwd={DB_PASS};Convert Zero Datetime=True";

//            return new MySqlConnection(conn);
//        }
//    }
//}

using MySql.Data.MySqlClient;
using System;

namespace konnect_player_info.Model.Manager
{
    public class BaseManager
    {
        public static string ConnectionString { get; set; }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}

