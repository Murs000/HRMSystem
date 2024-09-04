using HRMSystem.Business.DTOs;
using HRMSystem.Business.Services;
using HRMSystem.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "HR")]
public class HRController(IUserService userService) : ControllerBase
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
    public async Task<IActionResult> ExelExport()
    {
        // Export Workers Exel
        return Ok();
    }
}
