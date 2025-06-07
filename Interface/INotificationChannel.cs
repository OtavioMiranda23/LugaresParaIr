using LugaresParaIr.Dtos;

namespace LugaresParaIr.Interface;

public interface INotificationChannel
{
    Task SendAsyncEmail(NotificationMessageModel messageModel);
}