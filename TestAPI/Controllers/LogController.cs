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
    public class LogController : ControllerBase
    {
        private readonly ILogService _service;
        public LogController(ILogService services)
        {
            _service = services;
        }

        [HttpGet]
        public ActionResult<List<Log>> GetLog(string guid, string from, string till)
        {
            DateTime fromD;
            DateTime tillD;
            
            if (!DateTime.TryParse(from, out fromD))
                fromD = DateTime.Now.AddDays(-10);
            if (!DateTime.TryParse(from, out tillD))
                tillD = DateTime.Now.AddDays(10);

            List<Log> logs =_service.GetLogs(guid, fromD, tillD);
            return Ok(logs);
        }
    }
}