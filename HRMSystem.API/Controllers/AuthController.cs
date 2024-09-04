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
[AllowAnonymous]
public class AuthController(IUserService userService) : ControllerBase
{
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

    
}
