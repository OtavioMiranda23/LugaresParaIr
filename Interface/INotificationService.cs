namespace LugaresParaIr.Interface;

public interface INotificationService<T>
{
    Task<T> SendResetPassword(string emailTo, string token);
}