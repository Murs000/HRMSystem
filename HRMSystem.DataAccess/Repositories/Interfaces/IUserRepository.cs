using HRMSystem.DataAccess.Entities;
using System.Threading.Tasks;

namespace HRMSystem.DataAccess.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> GetByUsernameAsync(string username);
}
