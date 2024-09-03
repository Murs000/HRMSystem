using HRMSystem.DataAccess.Entities;
using HRMSystem.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace HRMSystem.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.PasswordSalt).IsRequired();
            builder.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(u => u.CreationDate).IsRequired();

            // Seed default admin user
            builder.HasData(new User
            {
                Id = 1,
                Username = "admin",
                // Password : admin
                PasswordHash = "f77982e85befc78e24e59d0080aa01834ec9a06e90b231c30d94f026c76e4576",
                PasswordSalt = "40cc50e45cba25c463a4130cd22e7e14",
                Role = UserRole.Admin.ToString(),
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
            });
        }
    }
}