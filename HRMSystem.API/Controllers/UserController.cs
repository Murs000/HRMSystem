using System.Security.Claims;
using HRMSystem.Business.DTOs;
using HRMSystem.Business.Services;
using HRMSystem.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        return Ok(await userService.GetCurrentUser());
    }
    [HttpGet("my-orders")]
    public IActionResult GetOrders()
    {
        // Get all users orders
        return Ok();
    }

    [HttpPost("ask-for-order")]
    public IActionResult AskOrder()
    {
        // Ask For Order
        return Ok();
    }
}
