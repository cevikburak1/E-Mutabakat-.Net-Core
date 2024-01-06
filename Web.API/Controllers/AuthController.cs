using Business.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegister userForRegister)
        {
            var userexist = _authService.UserExist(userForRegister.Email);
            if (!userexist.Success)
            {
                return BadRequest(userexist.Message);
            }
            //şifreyi ayrı gönderme sebebim şifreyi kriptoluyor olmamdan kaynaklı 
            var regsiterResult = _authService.Register(userForRegister, userForRegister.Password);
            var result = _authService.CreateAccessToken(regsiterResult.Data,0);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(regsiterResult.Message);

        }
        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var logindata = _authService.Login(userForLogin);
            if (!logindata.Success)
            {
                return BadRequest(logindata.Message);
            }
            var result = _authService.CreateAccessToken(logindata.Data,0);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }


    }
}
