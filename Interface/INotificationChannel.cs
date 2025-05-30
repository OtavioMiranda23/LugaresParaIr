using LugaresParaIr.Dtos;

namespace LugaresParaIr.Interface;

public interface INotificationChannel
{
    Task SendEmail(NotificationMessageModel messageModel);
}