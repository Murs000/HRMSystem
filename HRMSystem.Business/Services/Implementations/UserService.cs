using HRMSystem.Business.DTOs;
using HRMSystem.DataAccess.Entities;
using HRMSystem.DataAccess.Repositories;
using AutoMapper;
using System.Threading.Tasks;
using HRMSystem.DataAccess.Repositories.Interfaces;
using HRMSystem.Business.Services.Interfaces;

namespace HRMSystem.Business.Services.Implementations;

public class UserService(IUserRepository userRepository, IAuthenticationService authService) : IUserService
{
    public async Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDTO)
    {
        var user = await userRepository.GetByUsernameAsync(loginDTO.Username);

        if (user == null || authService.VerifyPassword(loginDTO.Password, user.PasswordHash, user.PasswordSalt) == false)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = authService.GenerateJwtToken(user.Id, user.Username, user.Role);

        return new UserResponseDTO
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token
        };
    }
    public async Task<UserDTO> CreateUserAsync(UserRegisterDTO userRegisterDTO, int adminId)
    {
        var hashedPassword = authService.HashPassword(userRegisterDTO.Password);

        var user = new User
        {
            Username = userRegisterDTO.Username,
            PasswordHash = hashedPassword.passwordHash,
            PasswordSalt = hashedPassword.passwordSalt,
            Role = userRegisterDTO.Role,
            CreatorId = adminId,  // Admin ID who created the user
            CreationDate = DateTime.UtcNow.AddHours(4)
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        
        return new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    // Get all users (Admin/HR only)
    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();

        var userDtos = new List<UserDTO>();
        foreach(var user in users)
        {
            var userDto = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
            };
            userDtos.Add(userDto);
        }
        return userDtos;
    }
}
