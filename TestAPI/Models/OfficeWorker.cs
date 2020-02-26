using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class OfficeWorker:Employee
    {
        public OfficeWorker()
        {
            Type = EmployeeType.Office;
        }
    }
}
