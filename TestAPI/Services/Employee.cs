using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;
using Microsoft.Data.Sqlite;
using System.Data;


namespace TestAPI.Services
{
    public class EmployeeService : IEmployeeServices
    {
        private string sqlrCOn = @"data source=test.db;";
        private SqliteConnection connection;
        public EmployeeService()
        {
            connection = new SqliteConnection(sqlrCOn);
        }

        public bool AddEmployee(Employee empl)
        {
            int RowCount=0;
            bool success = false;
            connection.Open();

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "Select Count(*) from Employee where login='" + empl.Login + "'";
            createTableCmd.CommandType = CommandType.Text;

            RowCount = Convert.ToInt32(createTableCmd.ExecuteScalar());
            
            if (RowCount != 0)
            {
                connection.Close();
                return success;
            }
            var insertCmd = connection.CreateCommand();
            System.Guid guid = System.Guid.NewGuid();

            insertCmd.CommandText = "insert into Employee (GUID ,dtcreated ,dtmodified ,dtdeleted , firstname ,lastname ,login ,password ,department ,role ,type ) "+
                                                    "values(@guid, @dtcreated,@dtmodified,@dtdeleted,@firstname,@lastname,@login,@password,@department,@role,@type)";
            insertCmd.CommandType = CommandType.Text;
            
            insertCmd.Parameters.AddWithValue("guid", guid.ToString());
            insertCmd.Parameters.AddWithValue("dtcreated", "" + DateTime.Now.Ticks);
            insertCmd.Parameters.AddWithValue("dtmodified", "" + DateTime.Now.Ticks);
            insertCmd.Parameters.AddWithValue("dtdeleted", "" + DateTime.MaxValue.Ticks);
            insertCmd.Parameters.AddWithValue("firstname", empl.FirstName);
            insertCmd.Parameters.AddWithValue("lastname", empl.LastName);
            insertCmd.Parameters.AddWithValue("login", empl.Login);
            insertCmd.Parameters.AddWithValue("password", "" + empl.Password);
            insertCmd.Parameters.AddWithValue("department", "" + empl.Department);
            insertCmd.Parameters.AddWithValue("role", "" + empl.Role);
            insertCmd.Parameters.AddWithValue("type", ""+empl.Type);

            if (insertCmd.ExecuteNonQuery() > 0)
                success = true;
            connection.Close();
            return success;
        }

        public bool DeleteEmployee(string OID)
        {
            connection.Open();
            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "update Employee set dtdeleted =@dtdeleted " +
                                                    " where guid=@guid";
            createTableCmd.CommandType = CommandType.Text;

            createTableCmd.Parameters.AddWithValue("dtdeleted", "" + DateTime.Now.Ticks);
            createTableCmd.Parameters.AddWithValue("guid", OID);

            createTableCmd.ExecuteNonQuery();
       
            int sqlite_datareader = createTableCmd.ExecuteNonQuery();
            connection.Close();
            return sqlite_datareader > 0;
        }

        public List<ProductionWorker> GetEmployees(string searchWord, string searchField, int limit, int page)
        {
            int RowCount = 0;
            connection.Open();
            var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getRecords.CommandText = "select  GUID GUID, firstname FirstName, lastname LastName, department Department, role Role, dtCreated DTCreated,dtModified DTModified,dtDeleted DTDeleted, login Login   from Employee where dtdeleted>'" + DateTime.Now.Ticks + "';";
            getTotalRecords.CommandText = "select Count(*)  from Employee where dtdeleted>"+DateTime.Now.Ticks + ";";
            if (limit > 0)
                getRecords.CommandText += " limit " + limit;
            if (limit > 0)
                getRecords.CommandText += " offset " + (page - 1) * limit;
            getRecords.CommandType = CommandType.Text;
            RowCount = Convert.ToInt32(getTotalRecords.ExecuteScalar());
            SqliteDataReader sqlite_datareader = getRecords.ExecuteReader();
            //sqlite_datareader.HasRows;
            List<ProductionWorker> typeData = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new ProductionWorker
                    {
                        GUID = (string)dr["GUID"],
                        FirstName = (string)dr["firstname"],
                        LastName = (string)dr["lastname"],
                        Department = (string)dr["department"],
                        Role = (string)dr["role"],
                        DTCreated = new DateTime(long.Parse(dr["dtCreated"].ToString())),
                        DTModified = new DateTime(long.Parse(dr["dtModified"].ToString())),
                        DTDeleted = new DateTime(long.Parse(dr["dtDeleted"].ToString())),
                        Login = (string)dr["login"]

                    }).ToList();
            return typeData;
        }
        /// <summary>
        /// Log in or log out Employee
        /// </summary>
        /// <param name="workerID"></param>
        /// <param name="toLogIn"></param>
        /// <returns></returns>
        public DateTime LogTime(string employeeID, bool toLogIn)
        {
            int RowCount = 0;
            if (IsLoggedIn(employeeID) && toLogIn)
                return DateTime.MinValue;
            connection.Open();
            //var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select Count(*)  from Employee where guid='"+ employeeID + "'";
            getTotalRecords.CommandType = CommandType.Text;
            RowCount = Convert.ToInt32(getTotalRecords.ExecuteScalar());
            if (RowCount > 0)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO Log VALUES('"+Guid.NewGuid().ToString()+"',"+DateTime.Now.Ticks + ",'"+ (toLogIn?LogType.Loggin:LogType.Logout)+"','"+ employeeID + "','')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "Update Employee set LastLoggedIn='"+DateTime.Now.Ticks + "' where guid='"+ employeeID + "'";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            connection.Close();
            return DateTime.Now;
        }
        /// <summary>
        /// Method check if Employee is logged in or not
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public bool IsLoggedIn(string employeeID)
        {
            bool loggedIn=false;
            connection.Open();
            
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select type  from Log where resource='" + employeeID + "' order by dtcreated DESC where type in (0,1)";
            getTotalRecords.CommandType = CommandType.Text;
            SqliteDataReader sqlite_datareader = getTotalRecords.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                loggedIn = sqlite_datareader.GetInt32(0) == 0;
                break;
            }
            
            connection.Close();

            return loggedIn;
        }
        public Employee UpdateEmployee(Employee empl)
        {
            connection.Open();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "update Employee set dtmodified =@dtmodified, firstname=@firstname ,lastname =@lastname , department=@department ,role=@role ,type =@type " +
                                                    " where login=@login";
            getTotalRecords.CommandType = CommandType.Text;

            
            getTotalRecords.Parameters.AddWithValue("dtmodified", "" + DateTime.Now.Ticks);
            getTotalRecords.Parameters.AddWithValue("firstname", empl.FirstName);
            getTotalRecords.Parameters.AddWithValue("lastname", empl.LastName);
            getTotalRecords.Parameters.AddWithValue("login", empl.Login);
            getTotalRecords.Parameters.AddWithValue("department", "" + empl.Department);
            getTotalRecords.Parameters.AddWithValue("role", "" + empl.Role);
            getTotalRecords.Parameters.AddWithValue("type", "" + empl.Type);

            getTotalRecords.ExecuteNonQuery();
            connection.Close();
            return empl;
        }

        public List<ShiftLog> LogWorkingHours(string employeeID)
        {
            connection.Open();
            
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select dtcreated DTCreated, type  from Log where resource='" + employeeID + "' order by dtcreated ASC where type in (0,1)";
            getTotalRecords.CommandType = CommandType.Text;
            SqliteDataReader sqlite_datareader = getTotalRecords.ExecuteReader();
            
            List<Log> logList = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new Log
                    {
                        OID = dr["guid"].ToString(),
                        Type = (LogType)int.Parse(dr["Type"].ToString()),
                        //Resource = (Employee)dr["Resource"],
                        Comment = (string)dr["Comment"],
                        DTCreated = new DateTime(long.Parse(dr["DTCreated"].ToString()))

                    }).ToList();

            connection.Close();
            List<ShiftLog> shiftLogList = new List<ShiftLog>();
            for (int a=0; a< logList.Count;a++)
            {
                ShiftLog sh = new ShiftLog();
                sh.TimeFrom = logList[a].DTCreated;
                if (logList[a + 1].Type == LogType.Logout)
                {
                    sh.TimeTill = logList[a + 1].DTCreated;
                    a++;
                }
                else
                    sh.TimeTill = sh.TimeTill.AddHours(9);// Temporary; TODO: add shift dictionary to get default working hours
                shiftLogList.Add(sh);
            }
            return shiftLogList;
        }

        public double GetHolidayBalance(string employeeID)
        {

            return 0;
        }
    }
}
