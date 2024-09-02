using System.Security.Cryptography;
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
        modelBuilder.Entity<User>().HasData(
            CreateAdminUser()
        );
        // Convert UserRole Enum to string in the database
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        // Global Query Filter for Soft Delete (IsDeleted)
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);
    }
    private User CreateAdminUser()
    {
        var password = "admin"; // Set a default password
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        return new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = UserRole.Admin,
            CreationDate = DateTime.UtcNow.AddHours(4),
            IsDeleted = false
        };
    }

    // Helper method to create a password hash and salt
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
