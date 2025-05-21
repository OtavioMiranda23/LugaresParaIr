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
    public string GenerateToken(UserModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        Console.WriteLine($"==========> {_secret}");
        var key = Encoding.ASCII.GetBytes(_secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Name.ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}