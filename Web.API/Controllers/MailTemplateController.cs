using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailTemplateController : ControllerBase
    {
        private readonly IMailTemplateService _mailTemplateService;

        public MailTemplateController(IMailTemplateService mailTemplateService)
        {
            _mailTemplateService = mailTemplateService;
        }
        [HttpPost("add")]
        public IActionResult Add(MailTemplate mailTemplate)
        {
            var result = _mailTemplateService.Add(mailTemplate);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

    }
}
