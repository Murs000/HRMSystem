using HRMSystem.DataAccess.Entities;
using HRMSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRMSystem.DataAccess.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly HRMDbContext _context;

    public UserRepository(HRMDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
    }
}
