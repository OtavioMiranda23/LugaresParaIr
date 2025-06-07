using LugaresParaIr.Data;
using LugaresParaIr.Services;
using LugaresParaIr.Services.Adapters;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LugaresParaIr.LugaresParaIr.Tests.Integration;

public class SendTemplateEmail
{
    public static AppDbContext CreateDbContext()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var connection = config.GetConnectionString("DefaultConnection");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connection)
            .Options;
        var context = new AppDbContext(options);
        return context;
    }
    [Fact]
    public void SuccessEmail()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets("9c9d4d88-25d0-4ad7-bdd6-7d0c15a8564c");
        var configuration = builder.Build();
        var apiKey = configuration["apiKey"];
        var apiSecret = configuration["secretKey"];
        if (apiKey == null || apiSecret == null)
        {
            throw new Exception("Api keys not found");
        }
        // var configApi = new Dictionary<string, string>
        // {
        //     { "apiKey", apiKey },
        //     { "apiSecret", apiSecret }
        // };
        // IConfiguration configurationMail =  new ConfigurationBuilder()
        //     .AddInMemoryCollection(configApi)
        //     .Build();
        var mailer = new MailJetAdapter(apiKey, apiSecret);
        var notificationService = new NotificationService(mailer, CreateDbContext());
        const string recipient = "lugaresparairsender@gmail.com";
        var exception = Record.ExceptionAsync(() =>  notificationService.SendResetPassword(recipient));
        Assert.Null(exception);
    }
    
}