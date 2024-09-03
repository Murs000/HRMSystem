namespace HRMSystem.Business.Services.Interfaces;

public interface IAuthenticationService
{
    (string passwordHash, string passwordSalt) HashPassword(string password);
    bool VerifyPassword(string password, string storedHash, string storedSalt);
    string GenerateJwtToken(int userId, string username, string role);
}
