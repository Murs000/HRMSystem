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
    public IActionResult GetCurrentUser()
    {
        // Extract the user ID from the JWT token in the HTTP context
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok($"Current User ID: {userId}");
    }
    [HttpGet("my-orders")]
    public IActionResult GetOrders()
    {
        // Get all users orders
        return Ok();
    }
}
