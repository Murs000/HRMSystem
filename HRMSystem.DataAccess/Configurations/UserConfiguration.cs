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
            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Surname).IsRequired().HasMaxLength(100);
            builder.Property(u => u.ExpirienceInCompany).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.PasswordSalt).IsRequired();
            builder.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired();
            builder.Property(u => u.CreationDate).IsRequired();

            builder.HasOne(u => u.Creator)
                .WithMany()
                .HasForeignKey(u => u.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // To avoid cascading delete
    
            builder.HasOne(u => u.Modifier)
                .WithMany()
                .HasForeignKey(u => u.ModifierId)
                .OnDelete(DeleteBehavior.Restrict); // To avoid cascading delete

            // Seed default admin user
            builder.HasData(new User
            {
                Id = 1,
                Username = "admin",
                Name = "Admin",
                Surname = "User",
                // Password : admin
                Role = UserRole.Admin.ToString(),
                IsDeleted = false,
                CreationDate = DateTime.UtcNow.AddHours(4),
            }.SetPassword("RLxOtsWawDlESSGFfMzkTYqlW5x11dGfGR0xB2LcRTg=", "40cc50e45cba25c463a4130cd22e7e14").SetAuditData(null));
        }
    }
}