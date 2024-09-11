using HRMSystem.Business.DTOs;
using HRMSystem.Business.Services;
using HRMSystem.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace HRMSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "HR")]
public class HRController(IExelService exelService,IWordService wordService) : ControllerBase
{
    [HttpGet("generate")]
    public async Task<IActionResult> GenerateDoc()
    {
        var fileData = await wordService.GenerateDocument();

        var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        var fileName = "UpdatedVacationOrder.docx";

        return File(fileData, contentType, fileName);
    }
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyDoc(IFormFile file)
    {
        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream); // Asynchronously copy the file content to memory
            fileData = memoryStream.ToArray(); // Convert the memory stream to a byte array
        }
        
        var result = await wordService.VerifyDocument(fileData);
        if(result)
        {
            return Ok("Veriried");
        }
        else
        {
            return BadRequest("Not Verified");
        }
    }
    [HttpGet("order-stats")]
    public async Task<IActionResult> OrderStats()
    {
        // Stats About how mutch orders each employee gets 
        // Dictionary<string,string> stats
        return Ok();
    }

    [HttpGet("workers-export")]
    [AllowAnonymous]
    public async Task<IActionResult> ExelExport()
    {
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = "WorkersReport.xlsx";

        var exelArray = await exelService.ExportWorkers();

        return File(exelArray.ToArray(), contentType, fileName);
    }
}
