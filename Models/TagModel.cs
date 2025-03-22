namespace LugaresParaIr.Models;

public class TagModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<LugarModel> Lugares { get; set; }    

}