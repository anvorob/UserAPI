using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Models;
using TestAPI.Services;
namespace TestAPI.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _service;
        public ShiftController(IShiftService services)
        {
            _service = services;
        }

        [HttpPost]
        public ActionResult AddShift(Shift shift)
        {
            if (_service.AddShift(shift))
                return Ok();
            else
                return BadRequest();
        }
        [HttpGet]
        public ActionResult<List<Shift>> GetShifts()
        {
            List<Shift> shiftList = new List<Shift>();
            shiftList = _service.GetShifts();
            return Ok(shiftList);
        }
        [HttpDelete]
        public ActionResult DeleteShift(string shiftID)
        {
            if (string.IsNullOrEmpty(shiftID))
                return BadRequest(new ReturnMessage("Shift id is not valid"));
            if(_service.DeleteShift(shiftID))
                return Ok();
            return BadRequest(new ReturnMessage("Failed to delete shift"));
        }
    }
}