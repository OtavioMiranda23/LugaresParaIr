using LugaresParaIr.Builder;
using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Exceptions;
using LugaresParaIr.Interface;
using LugaresParaIr.Models;

namespace LugaresParaIr.Services;

public class NotificationService : INotificationService
{
    private INotificationChannel _mailer;
    private readonly AppDbContext _context;

    public NotificationService(INotificationChannel mailer, AppDbContext context)
    {
        _context = context;
        _mailer = mailer;
    }

    public async Task SendResetPassword(string emailTo)
    {
        try
        {
            var template = await _context.TemplateEmail.FindAsync(3);
            if (template == null)
            {
                throw new TemplateNotFoundException("Template not found");
            }
            var emailMessage = new EmailBuilder()
                .SetFrom("lugaresparairsender@gmail.com")
                .SetTo(emailTo)
                .SetSubject(template.Subject)
                .SetBody(template.MessageTemplate)
                .SetLink("https://www.google.com/")
                .Build();
           await _mailer.SendAsyncEmail(emailMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}