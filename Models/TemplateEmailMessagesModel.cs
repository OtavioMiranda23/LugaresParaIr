using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LugaresParaIr.Models;

public class TemplateEmailMessagesModel
{
    public int Id { get; set; }
    [NotMapped]
    [Required(ErrorMessage = "O email é obrigatório.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email inválido")]
    public string To { get; set; }
    [Required(ErrorMessage = "O destinatário é obrigatório.")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "O template é obrigatório.")]
    public string  MessageTemplate { get; set; }
    [NotMapped]
    [Required(ErrorMessage = "O link é obrigatório.")]
    public string Link { get; set; }
}