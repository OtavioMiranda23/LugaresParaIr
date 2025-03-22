using System.ComponentModel.DataAnnotations;

namespace LugaresParaIr.Dtos;

public class TagCreateDto
{
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 30 caracteres.")]
    public string Name { get; set; }
}