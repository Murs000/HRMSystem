using HRMSystem.Business.DTOs;
using HRMSystem.DataAccess.Entities;
using HRMSystem.DataAccess.Repositories;
using AutoMapper;
using System.Threading.Tasks;
using HRMSystem.DataAccess.Repositories.Interfaces;
using HRMSystem.Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HRMSystem.Business.Services.Implementations;

public class UserService(IUserRepository userRepository, IAuthenticationService authService, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IUserService
{
    public async Task<UserDTO> GetCurrentUser()
    {
        // Extract the user ID from the JWT token in the HTTP context
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var users = await GetAllUsersAsync();
        var user = users.First(u=> u.Id == Convert.ToInt32(userId));

        return user;
    }

    public async Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDTO)
    {
        var user = await userRepository.GetByUsernameAsync(loginDTO.Username);

        if (user == null || authService.VerifyPassword(loginDTO.Password, user.PasswordHash, user.PasswordSalt) == false)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = authService.GenerateJwtToken(user.Id, user.Username, user.Role);

        var userResponceDTO = mapper.Map<UserResponseDTO>(user);
        userResponceDTO.Token = token;

        return userResponceDTO;
    }
    public async Task<UserDTO> CreateUserAsync(UserRegisterDTO userRegisterDTO, int adminId)
    {
        var hashedPassword = authService.HashPassword(userRegisterDTO.Password);

        var user = mapper.Map<User>(userRegisterDTO)
            .SetPassword(hashedPassword.passwordHash, hashedPassword.passwordSalt)
            .SetAuditData(adminId);

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        
        return mapper.Map<UserDTO>(user);
    }

    // Get all users (Admin/HR only)
    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();

        return mapper.Map<IEnumerable<UserDTO>>(users);
    }
}
