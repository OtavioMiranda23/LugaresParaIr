namespace LugaresParaIr.Dtos;

public class LugarDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Number { get; set; }
    public string Cep { get; set; }
    public CityZoneDetailsDto CityZoneDetails { get; set; }
    public bool? HasVisited { get; set; }
    public int? Avaliation { get; set; }
    public string Observation { get; set; }
    public List<TagDetailsDto> TagDetails { get; set; }
}

public class CityZoneDetailsDto
{
    public int? CityZoneId { get; set; }
    public string CityZone { get; set; }
}

public class TagDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}