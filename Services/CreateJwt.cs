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
    public string GenerateToken(UserModel user, int minutesToExpire)
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
            Expires = DateTime.UtcNow.AddMinutes(minutesToExpire),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}