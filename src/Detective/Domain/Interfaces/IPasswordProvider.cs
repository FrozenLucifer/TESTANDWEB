namespace Domain.Interfaces;

public interface IPasswordProvider
{
    string HashPassword(string password);
    bool VerifyPassword(string hash, string password);
    
    string GenerateTemporaryPassword(int length = 12);
}