using System.ComponentModel.DataAnnotations;
using LugaresParaIr.Enums;

namespace LugaresParaIr.Dtos;

public class LugaresCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres.")]
    public string? Address { get; set; }

    [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres.")]
    public string? Number { get; set; }

    [RegularExpression(@"\d{5}-\d{3}", ErrorMessage = "O CEP deve estar no formato 00000-000.")]
    public string? Cep { get; set; }

    [Required(ErrorMessage = "A zona da cidade é obrigatória.")]
    public CityZoneEnum CityZone { get; set; }

    public bool HasVisited { get; set; } = false;

    [Range(0, 5, ErrorMessage = "A avaliação deve estar entre 0 e 5.")]
    public int? Avaliation { get; set; } = 0;

    [StringLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Observation { get; set; }

    public List<int>? TagsIds { get; set; }  
}