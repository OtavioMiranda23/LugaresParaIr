using System.Text;
using System.Text.Json;
using LugaresParaIr.Dtos;
using Xunit;
using Xunit.Abstractions;

namespace LugaresParaIr.LugaresParaIr.Tests.e2e;

public class SendResetPassword
{
    private readonly ITestOutputHelper _output;
    public SendResetPassword(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public async void  Test()
    {
        //enviar requisição ao endpoint account/password
        string emailAddress = "teste@teste.com";
        StringContent jsonContentResetPassword = new(
            JsonSerializer.Serialize(new
            {
                Address = emailAddress
            }),
            Encoding.UTF8,
            "application/json");
        var httpClient = new HttpClient();
        string uri = "http://localhost:5030/api/usuario/account/password";
        HttpResponseMessage response = await httpClient.PutAsync(uri, jsonContentResetPassword);
        //guardar token
        var token = await response.Content.ReadAsStringAsync();
        Assert.NotNull(token);
        //enviar req ao endpoint account/password/recovery com nova senha
        StringContent jsonContentNewPassword = new(
            JsonSerializer.Serialize(new
            {
                Jwt = token,
                NewPassword = "Abc12345678"
            }),
            Encoding.UTF8,
            "application/json");
        uri = "http://localhost:5030/api/usuario/account/password/recovery";
        response = await httpClient.PutAsync(uri, jsonContentNewPassword);
        var userId = await response.Content.ReadAsStringAsync();
        Assert.NotNull(userId);
        //logar no endpoint de login com os novos dados e obter sucesso
        StringContent jsonContentLogin = new(
            JsonSerializer.Serialize(new
            {
                Email = emailAddress,
                    Password = "Abc12345678"
            }),
            Encoding.UTF8,
            "application/json");
        uri = "http://localhost:5030/api/usuario/login";
        response = await httpClient.PostAsync(uri, jsonContentLogin);
        string loginRaw = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("inválida", loginRaw);
        _output.WriteLine($"Login: => {loginRaw}");
        var loginJson = JsonSerializer.Deserialize<LoginReturnDto>(loginRaw);
        var userGuid = Guid.Parse(userId);
        Assert.Equal(loginJson.UserId, userGuid);
    }
    

}