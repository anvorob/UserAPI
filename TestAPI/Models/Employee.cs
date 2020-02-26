using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public enum EmployeeType{ 
        Production = 0, 
        Office =1
        }
    public abstract class Employee
    {
        public string GUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public DateTime DTCreated { get; set; }
        public DateTime DTModified { get; set; }
        public DateTime DTDeleted { get; set; }
        public string Role { get; set; }
        public EmployeeType Type { get; set; }


    }
}
