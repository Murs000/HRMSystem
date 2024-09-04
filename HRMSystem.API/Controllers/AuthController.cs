using HRMSystem.Business.DTOs;
using HRMSystem.Business.Services;
using HRMSystem.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("current-user")]
    public IActionResult GetCurrentUser1()
    {
        // Extract the user ID from the JWT token in the HTTP context
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok($"Current User ID: {userId}");
    }

    // Login action
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDTO loginDTO)
    {
        var result = await userService.LoginAsync(loginDTO);
        if (result == null)
        {
        return Unauthorized("Invalid login attempt.");
    }
    return Ok(result);
    }

    // Admin - create a new user
    [Authorize(Roles = "Admin")]
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(UserRegisterDTO userRegisterDTO)
    {
        // Extract admin's ID from the current HTTP context token
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
        // Call the service to create the user and assign adminId as the creator
        var createdUser = await userService.CreateUserAsync(userRegisterDTO, adminId);
        
        return Ok(createdUser);
    }

    // Admin or HR - Get all users
    [Authorize(Roles = "Admin,HR")]
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }
}
