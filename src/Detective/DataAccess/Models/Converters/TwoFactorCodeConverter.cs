using Domain.Models;

namespace DataAccess.Models.Converters;

public static class TwoFactorCodeConverter
{
    public static TwoFactorCode ToDomain(this TwoFactorCodeDb twoFactorCode)
    {
        return new TwoFactorCode(twoFactorCode.Username, twoFactorCode.Code, twoFactorCode.ExpiresAt, twoFactorCode.FailedAttempts);
    }
}