using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Services;
using TestAPI.Models;
namespace TestAPI.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeServices _service;
        public EmployeeController(IEmployeeServices services)
        {
            _service = services;
        }

        [HttpGet]
        [Route("Login")]
        public ActionResult<bool> Loggin(string OID)
        {
            //ProductionWorker worker = new ProductionWorker();
            DateTime loggedTime =_service.LogTime(OID, true);
            if (loggedTime == DateTime.MinValue)
                return Accepted(new ReturnMessage("Employee has already logged in"));
            return Ok();
        }

        [HttpGet]
        [Route("Logout")]
        public ActionResult<bool> Logout(string OID)
        {
            //ProductionWorker worker = new ProductionWorker();
            _service.LogTime(OID, false);
            return Ok();
        }

        [HttpPost]
        public ActionResult<Employee> AddEmployee(ProductionWorker empl)
        {
            //Employee em = new ProductionWorker();
            //ProductionWorker prod = new ProductionWorker();
            if(!_service.AddEmployee(empl))
                return BadRequest(new ReturnMessage("User with this login already exist"));

            return Ok(empl);
        }

        [HttpPut]
        public ActionResult<Employee> UpdateEmployee(ProductionWorker empl)
        {
            Employee returnedObj = _service.UpdateEmployee(empl);
            if (returnedObj != null)
                return Ok(empl);
            else
                return BadRequest();
        }

        [HttpDelete]
        public ActionResult DeleteEmployee(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return BadRequest();
            bool result=_service.DeleteEmployee(guid);
            if (result)
                return Ok();
            else
                return NotFound();
        }

        [HttpGet]
        public ActionResult<List<ProductionWorker>> GetEmployeeList(string searchWord="", string searchField = "", int limit = 10, int offset = 0)
        {
            List<ProductionWorker> returnList =_service.GetEmployees(searchWord, searchField, limit, offset);
            if (returnList != null)
                return Ok(returnList);
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("GetWorkHours")]
        public ActionResult<List<ShiftLog>> LogWorkingHours(string employeeID)
        {
            if(string.IsNullOrEmpty(employeeID))
                return BadRequest(new ReturnMessage("Employee ID is missing"));
            List<ShiftLog> shiftLog = new List<ShiftLog>();
            shiftLog = _service.LogWorkingHours(employeeID);
            return Ok(shiftLog);
        }
    }
}