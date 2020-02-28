using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;
using System.Data;
namespace TestAPI.Services
{
    public class LogService:ILogService
    {
        private string sqlrCOn = @"data source=test.db;";
        private SqliteConnection connection;
        public LogService()
        {
            connection = new SqliteConnection(sqlrCOn);
        }

        public void AddLog(Log log)
        {
            connection.Open();
            
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO Log VALUES('" + Guid.NewGuid().ToString() + "','" + DateTime.Now + "','" + log.Type + "','" + log.Resource + "','"+ log.Comment + "')";
            insertCmd.ExecuteNonQuery();
            connection.Close();
        }


        public List<Log> GetLogs(string guid, DateTime from, DateTime till)
        {
            connection.Open();

            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "Select DTCreated DTCreated,Type Type,Resource Resource,Comment Comment from Log where DTCreated between '" + from + "' and '" + till + "'";
            if(!string.IsNullOrEmpty(guid))
                insertCmd.CommandText +=" and Resource='"+guid+"';";
            insertCmd.ExecuteNonQuery();
            
            SqliteDataReader sqlite_datareader = insertCmd.ExecuteReader();
            //sqlite_datareader.HasRows;
            List<Log> typeData = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new Log
                    {
                        //Type = (LogType)dr["Type"],
                        //Resource = (Employee)dr["Resource"],
                        Comment = (string)dr["Comment"],
                        DTCreated = DateTime.Parse(dr["DTCreated"].ToString())
                        
                    }).ToList();
            connection.Close();
            return typeData;
        }

    }
}
