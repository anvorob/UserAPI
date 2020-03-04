using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    public class ShiftService : IShiftService
    {
        private string sqlrCOn = @"data source=test.db;";
        private SqliteConnection connection;
        public ShiftService()
        {
            connection = new SqliteConnection(sqlrCOn);
        }
        public bool AddShift(Shift shift)
        {
            bool success = false;
            connection.Open();

            var insertCmd = connection.CreateCommand();
            System.Guid guid = System.Guid.NewGuid();

            insertCmd.CommandText = "insert into Shift ( DayOfWeekFlags ,StartDate ,startTime ,Duration,EndTime,Category)" +
                                                    "values( @DayOfWeekFlags ,@StartDate ,@startTime ,@Duration,@EndTime,@Category )";
            insertCmd.CommandType = CommandType.Text;

            insertCmd.Parameters.AddWithValue("DayOfWeekFlags", shift.DayOfWeekFlags);
            insertCmd.Parameters.AddWithValue("StartDate", shift.StartDate);
            insertCmd.Parameters.AddWithValue("startTime", shift.StartTime);
            insertCmd.Parameters.AddWithValue("Duration", "" + shift.Duration);
            insertCmd.Parameters.AddWithValue("EndTime", "" + shift.EndTime);
            insertCmd.Parameters.AddWithValue("Category", "" + shift.Category);
            

            if (insertCmd.ExecuteNonQuery() > 0)
                success = true;
            connection.Close();
            return success;
        }

        public bool DeleteShift(string shiftID)
        {
            connection.Open();
            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "Delete Shift where OID=@OID";
            createTableCmd.CommandType = CommandType.Text;

            createTableCmd.Parameters.AddWithValue("OID", shiftID);

            int sqlite_datareader = createTableCmd.ExecuteNonQuery();
            connection.Close();
            return sqlite_datareader > 0;
        }

        public List<Shift> GetShifts()
        {
            int RowCount = 0;
            connection.Open();
            var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getRecords.CommandText = "select  DayOfWeekFlags DayOfWeekFlags, StartDate StartDate, StartTime StartTime, Duration Duration, Category Category from Shift;";
            getTotalRecords.CommandText = "select Count(*)  from Shift ;";
            
            getRecords.CommandType = CommandType.Text;
            RowCount = Convert.ToInt32(getTotalRecords.ExecuteScalar());
            SqliteDataReader sqlite_datareader = getRecords.ExecuteReader();

            List<Shift> typeData = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new Shift
                    {
                        DayOfWeekFlags = (DayOfWeekFlag)Enum.Parse(typeof(DayOfWeekFlag), dr["DayOfWeekFlags"].ToString()),
                        StartDate = DateTime.Parse(dr["StartDate"].ToString()),
                        StartTime = (TimeSpan)TimeSpan.Parse(dr["StartTime"].ToString()),
                        Duration = (TimeSpan)TimeSpan.Parse(dr["Duration"].ToString()),
                        Category = (EventCategory)Enum.Parse(typeof(EventCategory), dr["Category"].ToString())

                    }).ToList();
            return typeData;
        }

        public bool UpdateShift(Shift shift)
        {
            throw new NotImplementedException();
        }
    }
}
