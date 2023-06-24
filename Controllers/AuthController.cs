using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [EnableCors("apiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtManager _jwtManager;

        public AuthController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [HttpPost("Login")]
        public ActionResult GetToken(UserLoginBody user)
        {
            var token = _jwtManager.GenerateJWT(user);
            return Ok(token);
        }

        [HttpPost("Login/QR")]
        public ActionResult LoginViaQr()
        {
            throw new NotImplementedException();
        }
    }
}