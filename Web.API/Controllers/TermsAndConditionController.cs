using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermsAndConditionController : ControllerBase
    {
        private readonly ITermsAndConditionService _termsAndConditionService;

        public TermsAndConditionController(ITermsAndConditionService termsAndConditionService)
        {
            _termsAndConditionService = termsAndConditionService;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            var result = _termsAndConditionService.Get();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public IActionResult Update(TermsAndCondition termsAndCondition)
        {
            var result = _termsAndConditionService.Update(termsAndCondition);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
