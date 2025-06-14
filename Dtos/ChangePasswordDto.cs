using System.ComponentModel.DataAnnotations;

namespace LugaresParaIr.Dtos;

public class ChangePasswordDto
{
    [Required]
    public string Jwt;
    [Required]
    [MinLength(8)]
    public string NewPassword;
    
}