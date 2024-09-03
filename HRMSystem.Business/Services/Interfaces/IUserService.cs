using HRMSystem.Business.DTOs;
using System.Threading.Tasks;

namespace HRMSystem.Business.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> RegisterAsync(UserRegistrationDTO registrationDTO);
    Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDTO);
}
