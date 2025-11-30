using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface ITwoFactorRepository
{
    Task CreateCode(string username, string code);
    Task<TwoFactorCode?> FindOrDefaultCode(string username);
    Task DeleteCode(string username);
    Task UpdateFailedAttempts(string username, int newFailedAttempts);
}