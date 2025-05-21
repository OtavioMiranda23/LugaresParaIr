using System.ComponentModel.DataAnnotations;

namespace LugaresParaIr.Dtos;

public class Login
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email inválido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Senha inválida")]
    public string Password { get; set; }
}