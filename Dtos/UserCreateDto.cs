using System.ComponentModel.DataAnnotations;
using LugaresParaIr.Models;

namespace LugaresParaIr.Dtos;

public class UserCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter no máximo 60  e no minimo 3 caracteres")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O email é obrigatório.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email inválido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; }
}