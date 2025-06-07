namespace LugaresParaIr.Interface;

public interface INotificationService
{
    Task SendResetPassword(string emailTo);
}