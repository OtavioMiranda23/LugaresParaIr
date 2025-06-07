using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Interface;
using Mailjet.Client;
using Mailjet.Client.Resources.SMS;
using Mailjet.Client.TransactionalEmails;

namespace LugaresParaIr.Services.Adapters;

public class MailJetAdapter : INotificationChannel
{
    private MailjetClient Client;
    public MailJetAdapter(string apiKey, string apiSecret)
    {
        Client = new MailjetClient(apiKey, apiSecret);
    }
    
    public async Task SendAsyncEmail(NotificationMessageModel messageModel)
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
                throw new Exception("$Response do email vazio ou inválido! {messageModel.To}");
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao enviar email: {e}");
        }
    }
}