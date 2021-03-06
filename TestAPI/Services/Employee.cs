﻿using System;
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

        public bool AddEmployee(ProductionWorker empl)
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

            insertCmd.CommandText = "insert into Employee (GUID ,dtcreated ,dtmodified ,dtdeleted , firstname ,lastname ,login ,password ,department ,role ,type,rate ) "+
                                                    "values(@guid, @dtcreated,@dtmodified,@dtdeleted,@firstname,@lastname,@login,@password,@department,@role,@type,@rate)";
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
            insertCmd.Parameters.AddWithValue("rate", "" + empl.Rate);

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
            double EmployeeRate = 0;
            if (IsLoggedIn(employeeID) && toLogIn)
                throw new Exception("employee already logged in");

            if (IsOnLeave(employeeID))
                throw new Exception("Employee is on leave");
            IShiftService _shiftService = new ShiftService();
            List<Shift> shiftList = _shiftService.GetShifts();
            Shift ss = shiftList.FirstOrDefault(shft => shft.Category == EventCategory.WorkingTime);
            if (ss.StartTime > DateTime.Now.TimeOfDay && ss.EndTime < DateTime.Now.TimeOfDay)
                throw new Exception("loggin outside working hours");
            connection.Open();
            //var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select rate  from Employee where guid='"+ employeeID + "'";
            getTotalRecords.CommandType = CommandType.Text;
            EmployeeRate = Convert.ToDouble(getTotalRecords.ExecuteScalar());
            if (EmployeeRate > 0)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO Log VALUES('"+Guid.NewGuid().ToString()+"',"+
                                                                        DateTime.Now.Ticks + ",'"+ 
                                                                        (toLogIn?(int)LogType.Loggin: (int)LogType.Logout)+"','"+ 
                                                                        employeeID + "','"+  
                                                                        ((toLogIn)?EmployeeRate.ToString():"") + "')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "Update Employee set LastLoggedIn='"+DateTime.Now.Ticks + "' where guid='"+ employeeID + "'";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            connection.Close();
            return DateTime.Now;
        }

        private bool IsOnLeave(string employeeID)
        {
            
            connection.Open();

            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select *  from Log where resource='" + employeeID + "'and type = 2 and comment like '%[1]%' order by dtcreated DESC";
            getTotalRecords.CommandType = CommandType.Text;
            SqliteDataReader sqlite_datareader = getTotalRecords.ExecuteReader();
            List<HolidayLog> logList = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new HolidayLog((string)dr["Comment"])
                    {
                        OID = dr["guid"].ToString(),
                        Type = (LogType)int.Parse(dr["Type"].ToString()),
                        //Resource = (Employee)dr["Resource"],
                        //Comment = (string)dr["Comment"],
                        DTCreated = new DateTime(long.Parse(dr["DTCreated"].ToString()))
                    }).ToList();

            connection.Close();
            
            return logList.Exists(hlog => DateTime.Now.Date > hlog.DateFrom.Date && DateTime.Now.Date < hlog.DateTill);
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
            getTotalRecords.CommandText = "select type  from Log where resource='" + employeeID + "'and type in (0,1) and dtcreated>'"+DateTime.Now.Date.Ticks+"' order by dtcreated DESC";
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
        public Employee UpdateEmployee(ProductionWorker empl)
        {
            connection.Open();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "update Employee set dtmodified =@dtmodified, firstname=@firstname ,lastname =@lastname , department=@department ,role=@role ,type =@type, rate=@rate " +
                                                    " where login=@login";
            getTotalRecords.CommandType = CommandType.Text;

            getTotalRecords.Parameters.AddWithValue("dtmodified", "" + DateTime.Now.Ticks);
            getTotalRecords.Parameters.AddWithValue("firstname", empl.FirstName);
            getTotalRecords.Parameters.AddWithValue("lastname", empl.LastName);
            getTotalRecords.Parameters.AddWithValue("login", empl.Login);
            getTotalRecords.Parameters.AddWithValue("department", "" + empl.Department);
            getTotalRecords.Parameters.AddWithValue("role", "" + empl.Role);
            getTotalRecords.Parameters.AddWithValue("rate", "" + empl.Rate);
            getTotalRecords.Parameters.AddWithValue("type", "" + empl.Type);

            getTotalRecords.ExecuteNonQuery();
            connection.Close();
            return empl;
        }

        public List<ShiftLog> LogWorkingHours(string employeeID)
        {
            connection.Open();
            
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select guid guid, dtcreated DTCreated, type, comment  from Log where resource='" + employeeID + "' and type in (0,1) order by dtcreated ASC ;";
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

            IShiftService _shiftService = new ShiftService();
            List<Shift> shiftList = _shiftService.GetShifts();

            List<ShiftLog> shiftLogList = new List<ShiftLog>();
            for (int a=0; a< logList.Count;a++)
            {
                ShiftLog sh = new ShiftLog();
                sh.TimeFrom = logList[a].DTCreated;
                // Fetch rate for this shift from comments
                if (!string.IsNullOrEmpty(logList[a].Comment))
                    sh.Rate = double.Parse(logList[a].Comment);

                if (a!= logList.Count-1 &&  logList[a + 1].Type == LogType.Logout)
                {
                    // in case worker logs in and out we skip second iteration.
                    sh.TimeTill = logList[a + 1].DTCreated;
                    TimeSpan durationTmsp = sh.TimeTill.TimeOfDay - sh.TimeFrom.TimeOfDay;
                    sh.Hours = Math.Round(durationTmsp.TotalHours,2);
                    sh.DisplayHours = string.Format("{0}:{1}", durationTmsp.Hours, durationTmsp.Minutes);
                    a++;
                }
                else
                {
                    // If worker didnt log out set default date span
                    Shift ss = shiftList.FirstOrDefault(shft => shft.Category == EventCategory.WorkingTime);
                    TimeSpan durationTmsp = ss.EndTime - sh.TimeFrom.TimeOfDay;
                    sh.Hours = Math.Round(durationTmsp.TotalHours,2);
                    sh.DisplayHours = string.Format("{0}:{1}", durationTmsp.Hours, durationTmsp.Minutes);
                    sh.TimeTill = sh.TimeFrom.AddMinutes(durationTmsp.TotalMinutes);// Temporary; TODO: add shift dictionary to get default working hours
                }
                shiftLogList.Add(sh);
            }
            return shiftLogList;
        }

        public HolidayBalance GetHolidayBalance(string employeeID)
        {
            double minHolRate = 0.083475;
            List < ShiftLog> shiftList = this.LogWorkingHours(employeeID);
            List<HolidayLog> gg = GetLeaves(employeeID);
            HolidayBalance holdBal = new HolidayBalance();

            holdBal.TimeFrom = shiftList.OrderBy(s => s.TimeFrom).FirstOrDefault().TimeFrom;
            holdBal.TimeTill = shiftList.OrderBy(s => s.TimeFrom).LastOrDefault().TimeTill;// new DateTime();
            holdBal.HoursTotal= shiftList.Where(s=>!gg.Exists(hlog=> s.TimeFrom.Date>hlog.DateFrom.Date && s.TimeFrom.Date<hlog.DateTill)).Sum(s => s.Hours * minHolRate);
            holdBal.HoursUsed = gg.Where(hlog=>hlog.Approved).Sum(s => s.Hours);
            holdBal.HolidayPayTotal= shiftList.Sum(s => s.Hours * s.Rate * minHolRate);
            
            return holdBal;
        }

        public bool ApplyForLeave(string employeeID, string DateFrom, string DateTill)
        {
            connection.Open();
            //var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select rate  from Employee where guid='" + employeeID + "'";
            getTotalRecords.CommandType = CommandType.Text;
            
            if (Convert.ToDouble(getTotalRecords.ExecuteScalar()) > 0)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO Log VALUES('" + Guid.NewGuid().ToString() + "'," +
                                                                        DateTime.Now.Ticks + ",'" +
                                                                        LogType.PaidLeave + "','" +
                                                                        employeeID + "','[" +
                                                                        DateFrom + ":"+ DateTill + "][0]')";
                    insertCmd.ExecuteNonQuery();

                    //insertCmd.CommandText = "Update Employee set LastLoggedIn='" + DateTime.Now.Ticks + "' where guid='" + employeeID + "'";
                    //insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            connection.Close();
            return false;
        }

        public bool ApproveLeave(string leaveID)
        {
            throw new NotImplementedException();
        }

        public bool DenieLeave(string leaveID)
        {
            throw new NotImplementedException();
        }

        public List<HolidayLog> GetLeaves(string employeeID)
        {
            connection.Open();
            //var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select *  from Log where resource='" + employeeID + "' and type in ("+
                                                (int)LogType.PaidLeave+","+
                                                (int)LogType.UnpaidLeave + "," +
                                                (int)LogType.SickLeave+") and comment like '%[1]%'";
            getTotalRecords.CommandType = CommandType.Text;

            SqliteDataReader sqlite_datareader = getTotalRecords.ExecuteReader();

            List<HolidayLog> logList = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new HolidayLog((string)dr["Comment"])
                    {
                        OID = dr["guid"].ToString(),
                        Type = (LogType)int.Parse(dr["Type"].ToString()),
                        //Resource = (Employee)dr["Resource"],
                        //Comment = (string)dr["Comment"],
                        DTCreated = new DateTime(long.Parse(dr["DTCreated"].ToString()))
                    }).ToList();

            connection.Close();

            return logList;
        }
    }
}
