using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    interface IDepartment
    {
        Department AddDepartment(Department dept);
        Department UpdateDepartment(Department dept);
        bool DeleteDepartment(string OID);
        List<Department> GetListOfDepartments(string searchWord, string searchField, string limit, string page);
        Employee AddToDepartment(string emplOID, string deptOID);
    }
}
