namespace LugaresParaIr.Dtos;

public record LoginReturnDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
};