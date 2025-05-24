using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LugaresParaIr.Models;

public class UserModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter no máximo 60  e no minimo 3 caracteres")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O email é obrigatório.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email inválido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; }
    public ICollection<LugarModel> Lugares { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}