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
public class HRController(IExelService exelService) : ControllerBase
{
    [HttpGet("write-order")]
    public async Task<IActionResult> WriteOrder()
    {
        // Write Order from pattern and copy it to path
        return Ok();
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
