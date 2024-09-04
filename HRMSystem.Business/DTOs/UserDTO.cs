namespace HRMSystem.Business.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; } // Enum converted to string (Admin, HR, Employee)
}
