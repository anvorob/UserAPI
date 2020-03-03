using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Login is required field")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Password is required field")]
        public string Password { get; set; }
        public string Department { get; set; }
        public DateTime DTCreated { get; set; }
        public DateTime DTModified { get; set; }
        public DateTime DTDeleted { get; set; }
        public string Role { get; set; }
        public EmployeeType Type { get; set; }


    }
}
