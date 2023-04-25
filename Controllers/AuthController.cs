using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IJwtManager _jwtManager;

        public AuthController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [HttpPost("GetToken")]
        public ActionResult GetToken(UserDto user)
        {
            var token = _jwtManager.GenerateJWT(user);
            return Ok(token);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddNewUser")]
        public ActionResult AddUser(UserDto user)
        {
            _jwtManager.AddNewUser(user);
            return NoContent();
        }
    }
}