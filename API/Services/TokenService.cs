
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email)
        };

        // key for the token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["PrivateKey"] ?? 
            throw new InvalidOperationException("JWT private key is not configured (PrivateKey).")));
        // credential include algorithm and key
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // include all requires for the token (claims, (key, algorithm), expire date ....)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials
        };

        // token handler is that create and write the jwt string
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}