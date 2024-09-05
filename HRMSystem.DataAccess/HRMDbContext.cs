using System.Security.Cryptography;
using HRMSystem.DataAccess.Configurations;
using HRMSystem.DataAccess.Entities;
using HRMSystem.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRMSystem.DataAccess;

public class HRMDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public HRMDbContext(DbContextOptions<HRMDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
    // Migration Command: dotnet ef database update --startup-project ../HRMSystem.API
}
