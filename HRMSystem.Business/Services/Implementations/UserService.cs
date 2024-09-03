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

    public async Task<UserResponseDTO> RegisterAsync(UserRegistrationDTO registrationDTO)
    {
        var existingUser = await userRepository.GetByUsernameAsync(registrationDTO.Username);
        if (existingUser != null)
        {
            throw new ArgumentException("Username is already taken.");
        }

        var (passwordHash, passwordSalt) = authService.HashPassword(registrationDTO.Password);

        var user = new User
        {
            Username = registrationDTO.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = registrationDTO.Role,
            IsDeleted = false
        };

        user.SetCredentials();

        await userRepository.AddAsync(user);

        var token = authService.GenerateJwtToken(user.Id, user.Username, user.Role);

        return new UserResponseDTO
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token
        };
    }

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
}
