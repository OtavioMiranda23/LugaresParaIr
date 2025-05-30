using LugaresParaIr.Builder;
using LugaresParaIr.Dtos;
using LugaresParaIr.Interface;

namespace LugaresParaIr.Services;

public class NotificationService
{
    private INotificationChannel Mailer;
    public NotificationService(INotificationChannel mailer)
    {
        Mailer = mailer;
    }

    public void SendResetPassword()
    {
        try
        {
            //Acessar o body e subject no bd;
            var emailMessage = new EmailBuilder()
                .SetFrom("lugaresparairsender@gmail.com")
                .SetTo("lugaresparairsender@gmail.com")
                .SetSubject("Teste")
                .SetBody("<h1>Testandooooo</h1>")
                .Build();
            Mailer.SendEmail(emailMessage);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}