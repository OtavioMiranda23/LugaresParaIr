using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LugaresParaIr.Models;
using Microsoft.IdentityModel.Tokens;

namespace LugaresParaIr.Services;

public class CreateJwt
{
    private readonly string _secret;

    public CreateJwt(IConfiguration configuration)
    {
        _secret = configuration["User:Secret"];
    }
    public string GenerateToken(UserModel user, TimeSpan expiresIn)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Name.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString())
            }),
            Expires = DateTime.UtcNow.Add(expiresIn),
            NotBefore = DateTime.Now,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal ValidateJwt(string jwtToken)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken(jwtToken, parameters, out _);
    }
}