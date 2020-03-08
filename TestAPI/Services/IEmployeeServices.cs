using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    public interface IEmployeeServices
    {
        bool AddEmployee(ProductionWorker empl);
        Employee UpdateEmployee(ProductionWorker empl);
        bool DeleteEmployee(string OID);
        List<ProductionWorker> GetEmployees(string searchWord, string searchField, int limit, int page);
        DateTime LogTime(string workerID, bool toLogIn);
        bool IsLoggedIn(string workerID);
        List<ShiftLog> LogWorkingHours(string employeeID);
        HolidayBalance GetHolidayBalance(string employeeID);
        bool ApplyForLeave(string employeeID, string DateFrom, string DateTill);
        bool ApproveLeave(string leaveID);
        bool DenieLeave(string leaveID);
    }
}
