using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LugaresParaIr.Data;
using LugaresParaIr.Services;
using LugaresParaIr.Services.Adapters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    public async Task SendSuccessEmail()
    {
        // Carrega config do user secrets
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<SendTemplateEmail>() // Usa o mesmo tipo usado no secrets
            .Build();

        // Bind do MailJetSettings
        var services = new ServiceCollection();
        services.Configure<MailJetSettings>(configuration.GetSection("MailJetSettings"));
        var provider = services.BuildServiceProvider();

        var mailJetSettings = provider.GetRequiredService<IOptions<MailJetSettings>>().Value;

        if (string.IsNullOrWhiteSpace(mailJetSettings.ApiKey) || string.IsNullOrWhiteSpace(mailJetSettings.SecretKey))
        {
            throw new Exception("MailJet API keys não configuradas corretamente nos secrets.");
        }

        // Cria a instância do MailJetAdapter com IOptions
        var mailer = new MailJetAdapter(Options.Create(mailJetSettings));

        // Usa appsettings.Development.json para config adicional (como o frontend URL)
        var configurationDev = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var dbContext = CreateDbContext();

        var notificationService = new NotificationService(mailer, dbContext, configurationDev);

        const string recipient = "lugaresparairsender@gmail.com";
        var jwt = new CreateJwt(configuration);

        var user = dbContext.User.FirstOrDefault(u => u.Email == "teste@teste.com");

        if (user == null)
        {
            throw new Exception("Usuário não encontrado para envio de e-mail.");
        }

        var exception = await Record.ExceptionAsync(() =>
            notificationService.SendResetPassword(recipient, jwt.GenerateToken(user, TimeSpan.FromMinutes(2)))
        );
        Assert.Null(exception);
        var notification = await notificationService.SendResetPassword(recipient, jwt.GenerateToken(user, TimeSpan.FromMinutes(2)));
        var jwtToken = notification.Link.Split("token=")[1];
        
        //acessa o secret do jwt
        var secretJwt = configuration["User:Secret"];
        Assert.True(secretJwt != null);
        //valida se o jwt esta valido
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretJwt)),
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var exceptionToken = Record.Exception(() => tokenHandler.ValidateToken(jwtToken, parameters, out _));
        Assert.Null(exceptionToken);
    }
    
    [Fact]
    public async Task SendExpiredJwt()
    {
        // Carrega config do user secrets
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<SendTemplateEmail>() // Usa o mesmo tipo usado no secrets
            .Build();

        // Bind do MailJetSettings
        var services = new ServiceCollection();
        services.Configure<MailJetSettings>(configuration.GetSection("MailJetSettings"));
        var provider = services.BuildServiceProvider();

        var mailJetSettings = provider.GetRequiredService<IOptions<MailJetSettings>>().Value;

        if (string.IsNullOrWhiteSpace(mailJetSettings.ApiKey) || string.IsNullOrWhiteSpace(mailJetSettings.SecretKey))
        {
            throw new Exception("MailJet API keys não configuradas corretamente nos secrets.");
        }

        // Cria a instância do MailJetAdapter com IOptions
        var mailer = new MailJetAdapter(Options.Create(mailJetSettings));

        // Usa appsettings.Development.json para config adicional (como o frontend URL)
        var configurationDev = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var dbContext = CreateDbContext();

        var notificationService = new NotificationService(mailer, dbContext, configurationDev);

        const string recipient = "lugaresparairsender@gmail.com";
        var jwt = new CreateJwt(configuration);

        var user = dbContext.User.FirstOrDefault(u => u.Email == "teste@teste.com");

        if (user == null)
        {
            throw new Exception("Usuário não encontrado para envio de e-mail.");
        }

        var exception = await Record.ExceptionAsync(() =>
            notificationService.SendResetPassword(recipient, jwt.GenerateToken(user, TimeSpan.FromMinutes(2)))
        );
        Assert.Null(exception);
        var notification = await notificationService.SendResetPassword(recipient, jwt.GenerateToken(user, TimeSpan.FromMilliseconds(1)));
        var jwtToken = notification.Link.Split("token=")[1];
        
        //acessa o secret do jwt
        var secretJwt = configuration["User:Secret"];
        Assert.True(secretJwt != null);
        //valida se o jwt esta valido
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretJwt)),
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        Thread.Sleep(100);
        var exceptionToken = Record.Exception(() => tokenHandler.ValidateToken(jwtToken, parameters, out _));
        Assert.NotNull(exceptionToken);
    }
    
}