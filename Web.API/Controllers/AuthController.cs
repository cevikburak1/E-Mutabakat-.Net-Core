using Business.Abstract;
using Core.Utilities.Results.Concrete;
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
        public IActionResult RegisterSecondAccount(UserForRegisterSecondAccountDto userForRegisterSecondAccountDto)
        {
            var userexist = _authService.UserExist(userForRegisterSecondAccountDto.Email);
            if (!userexist.Success)
            {
                return BadRequest(userexist.Message);
            }
            //şifreyi ayrı gönderme sebebim şifreyi kriptoluyor olmamdan kaynaklı 
            var regsiterResult = _authService.RegisterSecondAccount(userForRegisterSecondAccountDto, userForRegisterSecondAccountDto.Password, userForRegisterSecondAccountDto.CompanyId);
            var result = _authService.CreateAccessToken(regsiterResult.Data, userForRegisterSecondAccountDto.CompanyId);
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
            if (logindata.Data.IsActive)
            {
                var usercompany = _authService.GetCompany(logindata.Data.Id).Data;
                var result = _authService.CreateAccessToken(logindata.Data, usercompany.CompanyId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);     
            }
            return BadRequest("Kullanıcı Pasif Durumda.Aktif Etmek için Yöneticiye Danışın");




        }


        [HttpGet("confirmuser")]
        public IActionResult ConfirmUser(string value)
        {
            var user = _authService.GetByMailConfirmValue(value).Data;
            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
           var result =  _authService.Update(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpGet("sendConfirmEmail")]
        public IActionResult SendConfirmEmail(int id)
        {
            var user = _authService.GetById(id).Data;
           var result = _authService.SendConfirmEmail(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
           
        }
    }
}
