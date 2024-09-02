using HRMSystem.DataAccess.Enums;
using HRMSystem.DataAccess.Common;

namespace HRMSystem.DataAccess.Entities;

public class User : EntityBase
{
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public UserRole Role { get; set; }  // Enum for User Roles    
}
