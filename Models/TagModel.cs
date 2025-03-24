using System.ComponentModel.DataAnnotations;

namespace LugaresParaIr.Models;

public class TagModel
{
    public int Id { get; set; }
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 30 caracteres.")]
    public string Name { get; set; }
    public ICollection<LugarModel> Lugares { get; set; }    

}