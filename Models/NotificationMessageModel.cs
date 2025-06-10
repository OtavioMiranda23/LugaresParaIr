using System.ComponentModel.DataAnnotations.Schema;

namespace LugaresParaIr.Dtos;

public class NotificationMessageModel()
{
    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    [NotMapped]
    public string Link { get; set; }
}