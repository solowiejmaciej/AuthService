using AuthService.Entities;
using AuthService.Exceptions;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    //[Authorize]
    [EnableCors("apiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: User/5

        [HttpGet("{Id}")]
        public UserDto GetById(string Id)
        {
            return _userService.GetById(Id);
        }

        // GET: User
        [HttpGet]
        public List<UserDto> GetAll()
        {
            return _userService.GetAll().Result;
        }

        // POST: User

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserBodyResponse>> AddNew([FromBody] UserBodyResponse user)
        {
            var newUserId = await _userService.AddAsync(user);
            return Created($"/api/User/{newUserId}", null);
        }

        // Delete: User/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}