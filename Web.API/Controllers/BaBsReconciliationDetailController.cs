using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaBsReconciliationDetailController : ControllerBase
    {
        private readonly IBaBsReconciliationDetailsService _baBsReconciliationDetailsService;

        public BaBsReconciliationDetailController(IBaBsReconciliationDetailsService baBsReconciliationDetailsService)
        {
            _baBsReconciliationDetailsService = baBsReconciliationDetailsService;
        }

        [HttpPost("add")]
        public IActionResult Add(BaBsReconciliationDetails baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailsService.Add(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public IActionResult Update(BaBsReconciliationDetails baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailsService.Update(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpPost("delete")]
        public IActionResult Delete(BaBsReconciliationDetails baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailsService.Delete(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id)
        {
            var result = _baBsReconciliationDetailsService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getList")]
        public IActionResult GetList(int baBsReconciliationId)
        {
            var result = _baBsReconciliationDetailsService.GetList(baBsReconciliationId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("addFromExcel")]
        public IActionResult AddFromExcel(IFormFile file, int baBsReconciliationId)
        {
            if (file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + ".xlsx";
                var filePath = $"{Directory.GetCurrentDirectory()}/Content/{fileName}";
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                    stream.Flush();
                }

                var result = _baBsReconciliationDetailsService.AddToExcel(filePath, baBsReconciliationId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Dosya seçimi yapmadınız");
        }
    }
}
