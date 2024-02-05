using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountReconciliationsController : ControllerBase
{
    private readonly IAccountReconciliationsService _accountReconciliationsService;

    public AccountReconciliationsController(IAccountReconciliationsService accountReconciliationsService)
    {
        _accountReconciliationsService = accountReconciliationsService;
    }

    [HttpPost("add")]
    public IActionResult Add(AccountReconciliations accountReconciliations)
    {
        var result = _accountReconciliationsService.Add(accountReconciliations);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

    [HttpPost("update")]
    public IActionResult Update(AccountReconciliations accountReconciliations)
    {
        var result = _accountReconciliationsService.Update(accountReconciliations);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }


    [HttpPost("delete")]
    public IActionResult Delete(AccountReconciliations accountReconciliations)
    {
        var result = _accountReconciliationsService.Delete(accountReconciliations);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

    [HttpGet("getbyid")]
    public IActionResult GetById(int id)
    {
        var result = _accountReconciliationsService.GetById(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

    [HttpGet("getList")]
    public IActionResult GetList(int companyId)
    {
        var result = _accountReconciliationsService.GetList(companyId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

    [HttpPost("addFromExcel")]
    public IActionResult AddFromExcel(IFormFile file, int companyId)
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

            var result = _accountReconciliationsService.AddToExcel(filePath, companyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        return BadRequest("Dosya seçimi yapmadınız");
    }

}
