using LugaresParaIr.Dtos;
using LugaresParaIr.Interface;
using Mailjet.Client;
using Mailjet.Client.Resources.SMS;
using Mailjet.Client.TransactionalEmails;

namespace LugaresParaIr.Services.Adapters;

public class MailJetAdapter : INotificationChannel
{
    private MailjetClient Client;
    private string ApiKey;
    private string ApiSecret;
    public MailJetAdapter(IConfiguration configuration)
    {
        ApiKey = configuration["User:apiKey"];
        ApiSecret = configuration["User:secretKey"];
        Client = new MailjetClient(ApiKey,ApiSecret);
    }
    
    public async Task SendEmail(NotificationMessageModel messageModel)
    {
        try
        {
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(messageModel.From))
                .WithSubject(messageModel.Subject)
                .WithHtmlPart(messageModel.Body)
                .WithTo(new SendContact(messageModel.To))
                .Build();
            var response = await Client.SendTransactionalEmailAsync(email);
            if (response.Messages != null && response.Messages.Length > 0)
            {
                Console.WriteLine("Email enviado com sucesso!");
            }
            else
            {
                Console.WriteLine($"Response do email vazio ou inválido! {messageModel.To}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao enviar email: {e}");
            throw;
        }
    }
}