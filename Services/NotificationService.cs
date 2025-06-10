using LugaresParaIr.Builder;
using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Exceptions;
using LugaresParaIr.Interface;
using LugaresParaIr.Models;

namespace LugaresParaIr.Services;

public class NotificationService : INotificationService<NotificationMessageModel>
{
    private INotificationChannel _mailer;
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    
    public NotificationService(INotificationChannel mailer, AppDbContext context, IConfiguration config)
    {
        _context = context;
        _mailer = mailer;
        _config = config;
    }

    public async Task<NotificationMessageModel> SendResetPassword(string emailTo, string token)
    {
        const int templateEmailNumber = (int) TemplateEmailEnum.ResetPassword;
        try
        {
            
            var template = await _context.TemplateEmail.FindAsync(templateEmailNumber);
            if (template == null)
            {
                throw new TemplateNotFoundException("Template not found");
            }
            //gerar token jwt, concatená-lo com o endereço
            var uri = _config["Logging:App:Frontend"];
            if (uri == null)
            {
                throw new Exception("Uri não encontrada");
            }
            
            var link = $"{uri}/passwordLost?token={token}"; 
            NotificationMessageModel emailMessage = new EmailBuilder()
                //TODO: Colocar em uma variavel de ambiente
                .SetFrom("lugaresparairsender@gmail.com")
                .SetTo(emailTo)
                .SetSubject(template.Subject)
                .SetBody(template.MessageTemplate)
                .SetLink(link)
                .Build();
           await _mailer.SendAsyncEmail(emailMessage);
           return emailMessage;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

enum TemplateEmailEnum
{
    ResetPassword = 3
}