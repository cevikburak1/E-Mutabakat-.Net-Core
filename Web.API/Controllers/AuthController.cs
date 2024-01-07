using Business.Abstract;
using Entities.Concrete;
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
        //API 2 Nesne Kabul etmedigi için  UserForRegister ve  Company i tek nesneye dönüştürüp kullandım
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegisterDto)
        {

            //kullanıcı bilgileri aldık şirket bilgilerini aldık  ikisinidekontrol ettirdim sistemde kayıtlı mı diye kayıtlı degilse gidip 1 kullanıcı 1 şirket kaydı oluşturdum
            var userexist = _authService.UserExist(userAndCompanyRegisterDto.UserForRegister.Email);
            if (!userexist.Success)
            {
                return BadRequest(userexist.Message);
            }
            var companyExist = _authService.CompanytExist(userAndCompanyRegisterDto.company);
            if (!companyExist.Success)
            {
                return BadRequest(userexist.Message);
            }

            //şifreyi ayrı gönderme sebebim şifreyi kriptoluyor olmamdan kaynaklı 
            var regsiterResult = _authService.Register(userAndCompanyRegisterDto.UserForRegister, userAndCompanyRegisterDto.UserForRegister.Password, userAndCompanyRegisterDto.company);
            var result = _authService.CreateAccessToken(regsiterResult.Data, regsiterResult.Data.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(regsiterResult.Message);

        }


        [HttpPost("registerSecondAccount")]
        public IActionResult RegisterSecondAccount(UserForRegister userForRegister, int companyId)
        {
            var userexist = _authService.UserExist(userForRegister.Email);
            if (!userexist.Success)
            {
                return BadRequest(userexist.Message);
            }
            //şifreyi ayrı gönderme sebebim şifreyi kriptoluyor olmamdan kaynaklı 
            var regsiterResult = _authService.RegisterSecondAccount(userForRegister, userForRegister.Password);
            var result = _authService.CreateAccessToken(regsiterResult.Data, 0);
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
