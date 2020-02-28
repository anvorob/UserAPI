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
            
            insertCmd.Parameters.AddWithValue("guid", guid.ToString());
            insertCmd.Parameters.AddWithValue("dtcreated", "" + DateTime.Now);
            insertCmd.Parameters.AddWithValue("dtmodified", "" + DateTime.Now);
            insertCmd.Parameters.AddWithValue("dtdeleted", "" + DateTime.MaxValue);
            insertCmd.Parameters.AddWithValue("firstname", empl.FirstName);
            insertCmd.Parameters.AddWithValue("lastname", empl.LastName);
            insertCmd.Parameters.AddWithValue("login", empl.Login);
            insertCmd.Parameters.AddWithValue("password", "" + empl.Password);
            insertCmd.Parameters.AddWithValue("department", "" + empl.Department);
            insertCmd.Parameters.AddWithValue("role", "" + empl.Role);
            insertCmd.Parameters.AddWithValue("type", ""+empl.Type);

            insertCmd.ExecuteNonQuery();

            connection.sqlite_conn.Close();
            return empl;
        }

        public bool DeleteEmployee(string OID)
        {
            connection.Open();
            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "update Employee set dtdeleted =@dtdeleted " +
                                                    " where guid=@guid";
            createTableCmd.CommandType = CommandType.Text;

            createTableCmd.Parameters.AddWithValue("dtdeleted", "" + DateTime.Now);
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
            getRecords.CommandText = "select  GUID GUID, firstname FirstName, lastname LastName, department Department, role Role, dtCreated DTCreated,dtModified DTModified,dtDeleted DTDeleted, login Login   from Employee where dtdeleted>'" + DateTime.Now + "';";
            getTotalRecords.CommandText = "select Count(*)  from Employee where dtdeleted>'"+DateTime.Now+"';";
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
                        DTCreated = DateTime.Parse(dr["dtCreated"].ToString()),
                        DTModified = DateTime.Parse(dr["dtModified"].ToString()),
                        DTDeleted = DateTime.Parse(dr["dtDeleted"].ToString()),
                        Login = (string)dr["login"]

                    }).ToList();
            return typeData;
        }

        public DateTime LogTime(string workerID, bool toLogIn)
        {
            int RowCount = 0;
            connection.Open();
            //var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getTotalRecords.CommandText = "select Count(*)  from Employee where guid='"+ workerID + "'";
            getTotalRecords.CommandType = CommandType.Text;
            RowCount = Convert.ToInt32(getTotalRecords.ExecuteScalar());
            if (RowCount > 0)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO Log VALUES('"+Guid.NewGuid().ToString()+"','"+DateTime.Now+"','"+ (toLogIn?LogType.Loggin:LogType.Logout)+"','"+ workerID + "','')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "Update Employee set LastLoggedIn='"+DateTime.Now+"' where guid='"+ workerID + "'";
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
            getTotalRecords.CommandText = "update Employee set dtmodified =@dtmodified, firstname=@firstname ,lastname =@lastname , department=@department ,role=@role ,type =@type " +
                                                    " where login=@login";
            getTotalRecords.CommandType = CommandType.Text;

            
            getTotalRecords.Parameters.AddWithValue("dtmodified", "" + DateTime.Now);
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

        
    }
}
