using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    }
}