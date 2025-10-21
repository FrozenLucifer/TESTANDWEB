using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Detective.Extensions;

public static class HttpContextExtensions{

    public static string GetUsername(this HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return null;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);
        
        return usernameClaim?.Value;
    }
}