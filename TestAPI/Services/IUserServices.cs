using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    public interface IUserServices
    {
        User AddUser(User user);
        ReturnObject GetUsers(int limit, int page);
        User GetUserByEmail(string email);
        bool DeleteUser(string email);
        User UpdateUser(User user);
        List<User> UserSearch(string searchWord,string searchField);
    }
}
