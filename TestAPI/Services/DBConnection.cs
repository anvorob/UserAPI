using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TestAPI.Services
{
    public class DBConnection
    {
        public SqliteConnection sqlite_conn;
        public DBConnection(string connectionStr)
        {
            if (string.IsNullOrEmpty(connectionStr))
                return;

            sqlite_conn = new SqliteConnection(connectionStr);
            
        }

        //List<string> stringLsit = new List<string>();
        //    // Open the connection:
        //    try
        //    {
        //        sqlite_conn.Open();

        //        SqliteCommand sqlite_cmd = sqlite_conn.CreateCommand();
        //        //sqlite_cmd = conn.CreateCommand();
        //        sqlite_cmd.CommandText = "SELECT * FROM tbl1";

        //        SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
        //        while (sqlite_datareader.Read())
        //        {
        //            string myreader = sqlite_datareader.GetString(0);
        //            stringLsit.Add(myreader);
        //        }
        //        sqlite_conn.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
    }
}
