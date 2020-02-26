using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestAPI.Models;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestAPI.Services
{
    public class UserService : IUserServices
    {
        
        private string sqlrCOn = @"data source=test.db;";
        private SqliteConnection connection;
        public UserService()
        {
            connection = new SqliteConnection(sqlrCOn);
            
        }
        public User AddUser(User user)
        {
            int RowCount = 0;

            DBConnection connection= new DBConnection(sqlrCOn);
            connection.sqlite_conn.Open();

            var createTableCmd = connection.sqlite_conn.CreateCommand();
            createTableCmd.CommandText = "Select Count(*) from User where email='"+ user.Email + "'";
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

            insertCmd.CommandText = "insert into User values('" + guid.ToString() + "', '" + user.Title + "','" + user.FirstName + "','" + user.LastName + "','" + user.Email + "','" + user.DOB + "','" + user.PhoneNumber + "')";
            insertCmd.ExecuteNonQuery();
            
            connection.sqlite_conn.Close();
            return user;

        }

        public ReturnObject GetUsers(int limit,int page = 1)
        {
            ReturnObject rObject = new ReturnObject();
            int RowCount = 0;
            //var connectionStringBuilder = new SqliteConnectionStringBuilder();

            ////Use DB in project directory.  If it does not exist, create it:
            //connectionStringBuilder.DataSource = "./SqliteDB.db";

            //using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            //{
            //    connection.Open();

            //    //Create a table (drop if already exists first):

            //    var delTableCmd = connection.CreateCommand();
            //    delTableCmd.CommandText = "DROP TABLE IF EXISTS favorite_beers";
            //    delTableCmd.ExecuteNonQuery();

            //    var createTableCmd = connection.CreateCommand();
            //    createTableCmd.CommandText = "CREATE TABLE favorite_beers(name VARCHAR(50))";
            //    createTableCmd.ExecuteNonQuery();

            //    //Seed some data:
            //    using (var transaction = connection.BeginTransaction())
            //    {
            //        var insertCmd = connection.CreateCommand();

            //        insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('LAGUNITAS IPA')";
            //        insertCmd.ExecuteNonQuery();

            //        insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('JAI ALAI IPA')";
            //        insertCmd.ExecuteNonQuery();

            //        insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('RANGER IPA')";
            //        insertCmd.ExecuteNonQuery();

            //        transaction.Commit();
            //    }

            //    //Read the newly inserted data:
            //    var selectCmd = connection.CreateCommand();
            //    selectCmd.CommandText = "SELECT name FROM favorite_beers";

            //    using (var reader = selectCmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            var message = reader.GetString(0);
            //            Console.WriteLine(message);
            //        }
            //    }


            //}

            connection.Open();
            var getRecords = connection.CreateCommand();
            var getTotalRecords = connection.CreateCommand();
            getRecords.CommandText = "select  OID OID,title Title, firstname FirstName, lastname LastName, email Email, dob DOB, phonenumber PhoneNumber  from User";
            getTotalRecords.CommandText = "select Count(*)  from User";
            if (limit > 0)
                getRecords.CommandText += " limit " + limit;
            if (limit > 0)
                getRecords.CommandText += " offset " + (page-1)*limit;
            getRecords.CommandType = CommandType.Text;
            RowCount = Convert.ToInt32(getTotalRecords.ExecuteScalar());
            SqliteDataReader sqlite_datareader = getRecords.ExecuteReader();
            //sqlite_datareader.HasRows;
            IEnumerable<User> typeData = sqlite_datareader.Cast<IDataRecord>()
                    .Select(dr => new User {
                        OID = (string)dr["OID"],
                        Title = (string)dr["title"],
                        FirstName = (string)dr["firstname"],
                        LastName = (string)dr["lastname"],
                        DOB = (string)dr["dob"],
                        PhoneNumber = (string)dr["phonenumber"],
                        Email = (string)dr["email"]
                    });

            
            if (page <= 0)
                page = 1;
            if (limit <= 0)
                limit = 1;


            rObject.CurrPage = page;
            rObject.Limit = limit;

            rObject.UserList = typeData.ToList();
            rObject.NumOfRecords = RowCount;
            rObject.TotalPages = (int)Math.Ceiling((decimal)RowCount / limit);
            connection.Close();
            
            return rObject;
        }

        public bool DeleteUser(string email)
        {
            
            connection.Open();

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "Delete from User where email='" + email + "'";
            createTableCmd.CommandType = CommandType.Text;

            int sqlite_datareader = createTableCmd.ExecuteNonQuery();
            connection.Close();
            return sqlite_datareader>0;
        }

        public User UpdateUser(User user)
        {
            int RowCount = 0;
            connection.Open();

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "Select Count(*) from User where email='" + user.Email + "'";
            createTableCmd.CommandType = CommandType.Text;

            SqliteDataReader sqlite_datareader = createTableCmd.ExecuteReader();
            //RowCount = Convert.ToInt32(createTableCmd.ExecuteScalar());
            connection.Close();
            if (!sqlite_datareader.HasRows)
                return null;
            else
            {
                //var dataClassList = connection.sqlite_conn.Query<User>(sql);   
            }
            return null;
            

            //User defObj = new User();
            //User oldUserObj = _userDictionary[user.Email];
            //Type userType = user.GetType();

            ////Get list of properties of 
            //List<PropertyInfo> props = new List<PropertyInfo>(userType.GetProperties());
            //foreach (PropertyInfo prop in props)
            //{
            //    // prevent overwriting OID
            //    if (prop.Name == "OID")
            //        continue;
            //    //Compare value of inbound object with default values
            //    //Update if values are different
            //    object newValue = prop.GetValue(user);
            //    object defaultValue = prop.GetValue(defObj);
            //    if (!object.Equals(newValue, defaultValue))
            //    {
            //        prop.SetValue(oldUserObj, prop.GetValue(user));
            //    }

            //}
            //_userDictionary[user.Email] = oldUserObj;
            //File.WriteAllText(_filepath, JsonConvert.SerializeObject(_userDictionary));
            //return oldUserObj;
        }

        public User GetUserByEmail(string email)
        {
            //if (!_userDictionary.ContainsKey(email))
            //    return null;

            //return _userDictionary[email];
            return null;
        }

        public List<User> UserSearch(string searchWord="",string searchField="")
        {
            if (new User().GetType().GetProperty(searchField) == null)
                return null;
            
            switch(searchField)
            {
                //case "Email": return _userDictionary.Where(ud => ud.Key.Contains(searchWord)).Select(ud=>ud.Value).ToList();
                //case "FirstName": return _userDictionary.Where(ud => ud.Value.FirstName!=null && ud.Value.FirstName.Contains(searchWord)).Select(ud => ud.Value).ToList();
                //case "LastName": return _userDictionary.Where(ud => ud.Value.LastName!=null && ud.Value.LastName.Contains(searchWord)).Select(ud => ud.Value).ToList();
                default: return null;

            }
            
        }

    }
}
