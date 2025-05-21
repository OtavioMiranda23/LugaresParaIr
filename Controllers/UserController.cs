using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Models;
using LugaresParaIr.Services;
using LugaresParaIr.Utils;
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
    public UserController(AppDbContext context, CreateJwt jwt)
    {
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
    //  TODO: COLOCAR SENHA EM UM .ENV E CRIAR ROLE PARA USERS 
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
        string tokenJwt = _jwt.GenerateToken(user);
        return Ok(new
        {
            userId = user.Id,
            token = tokenJwt
        });
    }
}