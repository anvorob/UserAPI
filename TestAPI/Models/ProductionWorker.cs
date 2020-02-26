using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class ProductionWorker : Employee
    {
        public ProductionWorker()
        {
            Type = EmployeeType.Production;
        }

        public DateTime LastLoggedIn { get; set; }
        public DateTime LoggedOut { get; set; }
    }
}
