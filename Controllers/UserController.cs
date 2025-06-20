using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Exceptions;
using LugaresParaIr.Interface;
using LugaresParaIr.Models;
using LugaresParaIr.Services;
using LugaresParaIr.Utils;
using LugaresParaIr.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LugaresParaIr.Controllers;

[Route("api/usuario")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CreateJwt _jwt;
    private readonly INotificationService<NotificationMessageModel> _notificationService;
    public UserController(AppDbContext context, CreateJwt jwt, INotificationService<NotificationMessageModel> notificationService)
    {
        _notificationService = notificationService;
        _context = context;
        _jwt = jwt;
    }
    [HttpPost("registrar")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto userData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new UserModel();
        var hashClass = new PasswordHasher<UserModel>();
        string hashValue = hashClass.HashPassword(user, userData.Password);
        user.Name = userData.Name;
        user.Email = userData.Email;
        user.Password = hashValue;
        _context.Add(user);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login loginData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = _context.User.FirstOrDefault(user => user.Email == loginData.Email);
        if (user == null)
        {
            return Unauthorized();
        }
        var hashClass = new PasswordHasher<UserModel>();
        var passwordVerification = hashClass.VerifyHashedPassword(user, user.Password, loginData.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }
        string tokenJwt = _jwt.GenerateToken(user, TimeSpan.FromMinutes(120));
        var loginDto = new LoginReturnDto
        {
            UserId = user.Id,
            Token = tokenJwt
        };
        return Ok(loginDto);

    }

    [HttpPut("account/password")]
    public async Task<IActionResult> ResetPassword([FromBody] EmailAddress emailAddress)
    {
        try
        {
            var email = new Email(emailAddress.Address);
            var user = await _context.User.Where(u => u.Email == email.Address).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Usuário não cadastrado");
            }

            string token = _jwt.GenerateToken(user, TimeSpan.FromMinutes(60));
            await _notificationService.SendResetPassword(email.Address, token);
            return Ok(token);
        }
        catch (TemplateNotFoundException e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
        }
        catch (InvalidEmailException e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("account/password/recovery")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dataChange)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var claimsPrincipal = _jwt.ValidateJwt(dataChange.Jwt);
            var claim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type.Contains("emailaddress"));
            if (claim == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Token inválido");
            }
            var email = claim.Value;
            var user = await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            //estando o jwt válido, preciso pegar o email que está no jwt buscar no banco, alterar senha hash
            var hashClass = new PasswordHasher<UserModel>();
            string hashValue = hashClass.HashPassword(user, dataChange.NewPassword);
            user.Password = hashValue;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user.Id.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Token inválido");
        }

    }
}