namespace HRMSystem.Business.DTOs;

public class UserRegistrationDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // Enum converted to string (Admin, HR, Employee)
}
