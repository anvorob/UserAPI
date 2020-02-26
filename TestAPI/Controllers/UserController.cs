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
    [Route("v1/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserServices _service;
        public UserController(IUserServices services)
        {
            _service = services;
        }
        [HttpPost]
        [Route("AddUserItem")]
        public ActionResult<User> AddUser(User user)
        {
            var users = _service.AddUser(user);
            
            if (users == null)
                return NotFound(new ReturnMessage("User with this email already exist"));
            return users;
        }
        [HttpPost]
        [Route("AddUserItems")]
        public ActionResult<List<User>> AddUsers(UserBatch userList)
        {
            List<User> returnUserObj = new List<User>();
            foreach(User user in userList.UserList)
                returnUserObj.Add(_service.AddUser(user));

            ReturnMessage rm = new ReturnMessage(string.Format("{0} records created", returnUserObj.Count));
            return Ok(rm);//TODO: Return list of successfully imported or failed users
        }

        [HttpGet]
        //[Route("GetUserItems")]
        public ActionResult<ReturnObject> GetUsers(int limit=5, int page=0)
        {
            return _service.GetUsers(limit, page);
            
        }
        [HttpDelete]
        [Route("DeleteUser")]
        public ActionResult DeleteUser(string email)
        {
            if (!string.IsNullOrEmpty(email) && _service.DeleteUser(email))
                return Ok(new ReturnMessage("Record deleted"));
            else
                return NotFound(new ReturnMessage( "Record doesnt exist" ));
        }

        [HttpPut]
        //[Route("UpdateUser")]
        public ActionResult<User> UpdateUser(User user)
        {
            return _service.UpdateUser(user);
        }

        [HttpGet]
        [Route("SearchUsers")]
        public ActionResult<List<User>> UserSearch(string searchWord,string searchField)
        {
            if (string.IsNullOrEmpty(searchWord) || string.IsNullOrEmpty(searchField))
                return BadRequest(new ReturnMessage("Search word or search field is not defined"));
            return _service.UserSearch(searchWord, searchField); 
        }

    }
}