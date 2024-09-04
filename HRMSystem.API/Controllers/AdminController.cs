using System.Security.Claims;
using HRMSystem.Business.DTOs;
using HRMSystem.Business.Services;
using HRMSystem.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(IUserService userService) : ControllerBase
{
    // Admin - create a new user
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
