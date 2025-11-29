using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class TwoFactorRepository : ITwoFactorRepository
{
    private readonly Context _context;

    public TwoFactorRepository(Context context)
    {
        _context = context;
    }

    
    public async Task CreateCode(string username, string code)
    {
        var existing = await _context.TwoFactorCodes
            .Where(x => x.Username == username)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            _context.TwoFactorCodes.Remove(existing);
        }

        var entity = new TwoFactorCodeDb(username: username,
            code: code,
            expiresAt: DateTime.UtcNow.AddMinutes(5),
            failedAttempts: 0);

        _context.TwoFactorCodes.Add(entity);
        await _context.SaveChangesAsync();
    }
    public async Task<TwoFactorCode?> FindOrDefaultCode(string username)
    {
        var codeDb = await _context.TwoFactorCodes
            .Where(x => x.Username == username)
            .FirstOrDefaultAsync();
        
        if (codeDb is null)
            return null;

        return codeDb.ToDomain();
    }

    public async Task DeleteCode(string username)
    {
        var code = await _context.TwoFactorCodes
            .Where(x => x.Username == username)
            .FirstOrDefaultAsync();

        if (code is not null)
        {
            _context.TwoFactorCodes.Remove(code);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateFailedAttempts(string username, int newFailedAttempts)
    {
        var code = await _context.TwoFactorCodes
            .Where(x => x.Username == username)
            .FirstOrDefaultAsync();

        if (code is not null)
        {
            code.FailedAttempts = newFailedAttempts;
            await _context.SaveChangesAsync();
        }
    }
}