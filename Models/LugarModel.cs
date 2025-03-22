using System.Text.Json.Serialization;
using LugaresParaIr.Enums;

namespace LugaresParaIr.Models;

public class LugarModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Number { get; set; }
    public string Cep { get; set; }
    public CityZoneEnum CityZone { get; set; }
    public bool HasVisited { get; set; }
    public int Avaliation { get; set; }
    public string Observation { get; set; }
    [JsonIgnore]
    public ICollection<TagModel> Tags { get; set; }    
}