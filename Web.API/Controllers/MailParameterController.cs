using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailParameterController : ControllerBase
    {
        private readonly IMailParameterService _mailParameterService;

        public MailParameterController(IMailParameterService mailParameterService)
        {
            _mailParameterService = mailParameterService;
        }
        [HttpPost("update")]
        public IActionResult MailParameter(MailParameter mailParameter)
        {
            var result = _mailParameterService.Update(mailParameter);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

    }
}
