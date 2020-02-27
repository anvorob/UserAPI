using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;
using Microsoft.Data.Sqlite;
using System.Data;
//using System.Data.SQLite;
//using System.Data.SqlClient;

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

        public Employee AddEmployee(Employee empl)
        {
            int RowCount=0;
            DBConnection connection = new DBConnection(sqlrCOn);
            connection.sqlite_conn.Open();

            var createTableCmd = connection.sqlite_conn.CreateCommand();
            createTableCmd.CommandText = "Select Count(*) from Employee where login='" + empl.Login + "'";
            createTableCmd.CommandType = CommandType.Text;

            RowCount = Convert.ToInt32(createTableCmd.ExecuteScalar());
            //createTableCmd.ExecuteNonQuery();
            //SqliteDataReader sqlite_datareader = createTableCmd.ExecuteReader();
            if (RowCount != 0)
            {
                connection.sqlite_conn.Close();
                return null;
            }
            var insertCmd = connection.sqlite_conn.CreateCommand();
            System.Guid guid = System.Guid.NewGuid();

            insertCmd.CommandText = "insert into Employee (GUID ,dtcreated ,dtmodified ,dtdeleted , firstname ,lastname ,login ,password ,department ,role ,type ) "+
                "values(@guid, @dtcreated,@dtmodified,@dtdeleted,@firstname,@lastname,@login,@password,@department,@role,@type)";
            insertCmd.CommandType = CommandType.Text;
            //insertCmd.Parameters.Add(new SqlParameter("@guid", guid.ToString()));
            //insertCmd.Parameters.Add(new SqlParameter("@dtcreated", DateTime.Now));
            //insertCmd.Parameters.Add(new SqlParameter("@dtmodified", DateTime.Now));
            //insertCmd.Parameters.Add(new SqlParameter("@dtdeleted", DateTime.MaxValue));
            //insertCmd.Parameters.Add(new SqlParameter("@firstname", empl.FirstName));
            //insertCmd.Parameters.Add(new SqlParameter("@lastname", empl.LastName));
            //insertCmd.Parameters.Add(new SqlParameter("@login", empl.Login));
            //insertCmd.Parameters.Add(new SqlParameter("@password", empl.Password));
            //insertCmd.Parameters.Add(new SqlParameter("@department", empl.Department));
            //insertCmd.Parameters.Add(new SqlParameter("@role", empl.Role));
            //insertCmd.Parameters.Add(new SqlParameter("@type", empl.Type));

            insertCmd.Parameters.AddWithValue("guid", guid.ToString());
            insertCmd.Parameters.AddWithValue("dtcreated", DateTime.Now);
            insertCmd.Parameters.AddWithValue("dtmodified", DateTime.Now);
            insertCmd.Parameters.AddWithValue("dtdeleted", DateTime.MaxValue);
            insertCmd.Parameters.AddWithValue("firstname", empl.FirstName);
            insertCmd.Parameters.AddWithValue("lastname", empl.LastName);
            insertCmd.Parameters.AddWithValue("login", empl.Login);
            insertCmd.Parameters.AddWithValue("password", empl.Password);
            insertCmd.Parameters.AddWithValue("department", empl.Department);
            insertCmd.Parameters.AddWithValue("role", empl.Role);
            insertCmd.Parameters.AddWithValue("type", empl.Type);

            //insertCmd.Parameters.AddWithValue("$lastloggedin", empl.);
            //insertCmd.Parameters.AddWithValue("$loggedout", empl.FirstName);

            insertCmd.ExecuteNonQuery();

            connection.sqlite_conn.Close();
            return empl;
        }

        public bool DeleteEmployee(string OID)
        {
            connection.Open();

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "Delete from Employee where guid='" + OID + "'";
            createTableCmd.CommandType = CommandType.Text;

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
            getRecords.CommandText = "select  GUID GUID, firstname FirstName, lastname LastName, department Department, role Role  from Employee";
            getTotalRecords.CommandText = "select Count(*)  from Employee";
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
                        Role = (string)dr["role"]

                    }).ToList();
            return typeData;
        }

        public DateTime LogTime(string workerID, bool toLogIn)
        {
            int RowCount = 0;
            connection.Open();
            //var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            //getRecords.CommandText = "select  OID OID,title Title, firstname FirstName, lastname LastName, email Email, dob DOB, phonenumber PhoneNumber  from User";
            getTotalRecords.CommandText = "select Count(*)  from User where OID='"+ workerID + "''";
            RowCount = Convert.ToInt32(getTotalRecords.ExecuteScalar());
            if (RowCount > 0)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO Log VALUES('"+Guid.NewGuid().ToString()+"','"+DateTime.Now+"','"+ (toLogIn?LogType.Loggin:LogType.Logout)+"','"+ workerID + "','')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "Update Employee set LastLoggedIn='"+DateTime.Now+"' where OID='"+ workerID + "'";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            connection.Close();
            return DateTime.Now;
        }

        public Employee UpdateEmployee(Employee empl)
        {
            connection.Open();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "UPDATE Employee SET dname = :dname, loc = :loc WHERE deptno = @deptno";
            //getTotalRecords.Parameters.Add("param1", 30);
            //getTotalRecords.Parameters.Add("param2", "SALES");
            //getTotalRecords.Parameters.Add("param3", "CHICAGO");
            getTotalRecords.ExecuteNonQuery();
            connection.Close();
            return empl;
        }

        
    }
}
