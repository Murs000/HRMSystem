using HRMSystem.Business.DTOs;
using System.Threading.Tasks;

namespace HRMSystem.Business.Services.Interfaces;

public interface IUserService
{
    public Task<UserResponseDTO> LoginAsync(UserLoginDTO loginDTO);
    // Admin creates user
    public Task<UserDTO> CreateUserAsync(UserRegisterDTO userRegisterDTO, int adminId);

    // Get all users (Admin/HR only)
    public Task<IEnumerable<UserDTO>> GetAllUsersAsync();
}
