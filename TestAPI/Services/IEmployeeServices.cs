using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    public interface IEmployeeServices
    {
        bool AddEmployee(Employee empl);
        Employee UpdateEmployee(Employee empl);
        bool DeleteEmployee(string OID);
        List<ProductionWorker> GetEmployees(string searchWord, string searchField, int limit, int page);
        DateTime LogTime(string workerID, bool toLogIn);
        bool IsLoggedIn(string workerID);
    }
}
