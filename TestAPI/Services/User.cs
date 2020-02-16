using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    public class UserService : IUserServices
    {
        private readonly Dictionary<string, User> _userDictionary;
        private string _filepath = "../json1.json";
        public UserService()
        {
            
            string result = string.Empty;
            _userDictionary = new Dictionary<string, User>();
            if (!File.Exists(_filepath))
                return;

            using (StreamReader r = new StreamReader(_filepath))
            {
                var json = r.ReadToEnd();
                _userDictionary = JsonConvert.DeserializeObject<Dictionary<string, User>>(json);
                
            }
            
        }
        public User AddUser(User user)
        {
            int lastIndex = 0;
            if(_userDictionary.Count>0)
                lastIndex=_userDictionary.OrderBy(ud => ud.Value.OID).LastOrDefault().Value.OID;
            user.OID = lastIndex + 1;

            if (!_userDictionary.ContainsKey(user.Email))
            {
                _userDictionary.Add(user.Email, user);
                File.WriteAllText(_filepath, JsonConvert.SerializeObject(_userDictionary));
            }
            else
                return null;
            return user;
        }

        public ReturnObject GetUsers(int limit,int page = 1)
        {
            ReturnObject rObject = new ReturnObject();

            if (page <= 0)
                page = 1;
            if (limit <= 0)
                limit = 1;


            rObject.CurrPage = page;
            rObject.Limit = limit;

            rObject.UserList = _userDictionary.OrderBy(ud => ud.Value.OID).Skip(limit*(page-1)).Take(limit).Select(ud=>ud.Value).ToList();
            rObject.NumOfRecords = _userDictionary.Count;
            rObject.TotalPages = (int)Math.Ceiling((decimal)_userDictionary.Count / limit);
            return rObject;
        }

        public bool DeleteUser(string email)
        {
            bool deleteResult = _userDictionary.Remove(email);
            File.WriteAllText(_filepath, JsonConvert.SerializeObject(_userDictionary));
            return deleteResult;
        }

        public User UpdateUser(User user)
        {
            if (!_userDictionary.ContainsKey(user.Email))
                return null;

            User defObj = new User();
            User oldUserObj = _userDictionary[user.Email];
            Type userType = user.GetType();

            //Get list of properties of 
            IList<PropertyInfo> props = new List<PropertyInfo>(userType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                // prevent overwriting OID
                if (prop.Name == "OID")
                    continue;
                //Compare value of inbound object with default values
                //Update if values are different
                object newValue = prop.GetValue(user);
                object defaultValue = prop.GetValue(defObj);
                if (!object.Equals(newValue, defaultValue))
                {
                    prop.SetValue(oldUserObj, prop.GetValue(user));
                }

            }
            _userDictionary[user.Email] = oldUserObj;
            File.WriteAllText(_filepath, JsonConvert.SerializeObject(_userDictionary));
            return oldUserObj;
        }

        public User GetUserByEmail(string email)
        {
            if (!_userDictionary.ContainsKey(email))
                return null;

            return _userDictionary[email];
        }

        public List<User> UserSearch(string searchWord="",string searchField="")
        {
            if (new User().GetType().GetProperty(searchField) == null)
                return null;
            
            switch(searchField)
            {
                case "Email": return _userDictionary.Where(ud => ud.Key.Contains(searchWord)).Select(ud=>ud.Value).ToList();
                case "FirstName": return _userDictionary.Where(ud => ud.Value.FirstName!=null && ud.Value.FirstName.Contains(searchWord)).Select(ud => ud.Value).ToList();
                case "LastName": return _userDictionary.Where(ud => ud.Value.LastName!=null && ud.Value.LastName.Contains(searchWord)).Select(ud => ud.Value).ToList();
                default: return null;

            }
            
        }

    }
}
