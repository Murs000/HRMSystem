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
    public string PasswordHash { get; set; }

    [Required]
    public string PasswordSalt { get; set; }

    [Required]
    public string Role { get; set; }
}
