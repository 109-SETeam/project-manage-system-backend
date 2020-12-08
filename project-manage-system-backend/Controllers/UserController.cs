﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(PMSContext dbContext)
        {
            _userService = new UserService(dbContext);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(_userService.GetUser(User.Identity.Name));
        }

        [Authorize]
        [HttpGet("admin")]
        public IActionResult GetAllUser()
        {
            return Ok(_userService.GetAllUserExceptAdmin(User.Identity.Name));
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteUser(string account)
        {
            try
            {
                if (User.Identity.AuthenticationType == "Admin")
                {
                    _userService.DeleteUserByAccount(account);
                    return Ok(new ResponseDto { success = true, message = "delete success!" });
                }
            }
            catch (System.Exception e)
            {
                return Ok(new ResponseDto { success = false, message = $"delete fail：{e.Message}" });
            }
            return BadRequest();
        }
    }
}
