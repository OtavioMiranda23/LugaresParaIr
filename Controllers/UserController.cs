using System.Net;
using System.Security.Cryptography.X509Certificates;
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
using Microsoft.EntityFrameworkCore;

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
    [HttpPost("Login")]
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
        return Ok(new
        {
            userId = user.Id,
            token = tokenJwt
        });
        
    }

    [HttpPut("account/password")]
    public async Task<IActionResult> ResetPassword([FromBody] string emailAddress)
    {
        try
        {
            var email = new Email(emailAddress);
            var user = await _context.User.Where(u => u.Email == email.Address).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Usuário não cadastrado");
            }

            string token = _jwt.GenerateToken(user, TimeSpan.FromMinutes(60));
            await _notificationService.SendResetPassword(email.Address, token);
            return Ok();
        }
        catch (TemplateNotFoundException e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
        }
        catch (InvalidEmailException e)
        {
            return BadRequest(e.Message);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword([FromBody] string jwt,  string newPassword)
        {
            
            dataChange.Jwt;
            return Ok();
        }
        
    }
}