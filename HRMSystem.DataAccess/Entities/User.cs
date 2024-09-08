using HRMSystem.DataAccess.Enums;
using HRMSystem.DataAccess.Common;
using System.ComponentModel.DataAnnotations;

namespace HRMSystem.DataAccess.Entities;

public class User : EntityBase
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Surname { get; set; }
    [Required]
    public int ExperienceInCompany { get; set; }

    [Required]
    public string PasswordHash { get; private set; }

    [Required]
    public string PasswordSalt { get; private set; }

    [Required]
    public string Role { get; set; }

    public User SetPassword(string passwordHash, string passwordSalt)
    {
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;

        return this;
    }
    // Method to set credentials for audit
    public User SetAuditData(int? adminId)
    {
        this.SetCredentials(adminId);
        return this;
    }
}
