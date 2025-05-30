using System.ComponentModel.DataAnnotations;
using LugaresParaIr.Models;
using Mailjet.Client;
using Mailjet.Client.Resources.SMS;
using Mailjet.Client.TransactionalEmails;
using Xunit;

namespace LugaresParaIr.LugaresParaIr.Tests;

public class TemplateEmailMessageModelTest
{
    [Fact]
    public void SuccessTemplateEmailMessageTest()
    {
        var model = new TemplateEmailMessagesModel
        {
            To = "email_valido@gmail.com",
            Subject = "Assunto",
            MessageTemplate = 1,
            Link = "https://exemplo.com"
        };

        var erros = new List<ValidationResult>();
        var valido = Validator.TryValidateObject(model, new ValidationContext(model), erros, true);

        Assert.True(valido);
        // Assert.Contains(erros, e => e.ErrorMessage.Contains("Email inválido"));
    }
    
    [Fact]
    public void SholdInvalidTemplateEmailMessageTest()
    {
        var model = new TemplateEmailMessagesModel
        {
            To = "email_validogmail.com",
            Subject = "Assunto",
            MessageTemplate = 1,
            Link = "https://exemplo.com"
        };

        var erros = new List<ValidationResult>();
        var valido = Validator.TryValidateObject(model, new ValidationContext(model), erros, true);

        Assert.False(valido);
        Assert.Contains(erros, e => e.ErrorMessage.Contains("Email inválido"));
    }
    
    [Fact]
    public async void TesteEmail()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets<TemplateEmailMessageModelTest>();
        var configuration = builder.Build();
        
        var apiKey = configuration["apiKey"];
        var apiSecret = configuration["secretKey"];
        Console.WriteLine(apiKey);
        Console.WriteLine(apiSecret);
        MailjetClient client = new MailjetClient(
            apiKey,
            apiSecret);

        MailjetRequest request = new MailjetRequest
        {
            Resource = Send.Resource
        };
        var email = new TransactionalEmailBuilder()
            .WithFrom(new SendContact("lugaresparairsender@gmail.com"))
            .WithSubject("Teste subject")
            .WithHtmlPart("<h1>Ola!</h1>")
            .WithTo(new SendContact("lugaresparairsender@gmail.com"))
            .Build();
        var response = await client.SendTransactionalEmailAsync(email);
        Assert.Equal(1, response.Messages.Length);
    } 
}
