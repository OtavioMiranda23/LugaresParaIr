using System.Text.Json;
using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LugaresParaIr.Controllers;

[Route("api/lugares")]
public class LugaresController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public  LugaresController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetLugares()
    {
        var lugares = await _context.Lugares
            .Include(lugar => lugar.Tags)
            .Select(lugar => new
            {
                lugar.Id,
                lugar.Name,
                lugar.Address,
                lugar.Number,
                lugar.Cep,
                lugar.CityZone,
                lugar.HasVisited,
                lugar.Avaliation,
                lugar.Observation,
                TagDetails = lugar.Tags.Select(t => new
                {
                    t.Id,
                    t.Name
                }).ToList()
            }).ToListAsync();
        return Ok(lugares);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLugarById(int id)
    {
        var lugar = await _context.Lugares
            .Where(lugar => lugar.Id == id)
            .Include(lugar => lugar.Tags) // Faz join com a tabela de Tags, traz suas tags associadas 
            .Select(lugar => new
            {
                lugar.Id,
                lugar.Name,
                TagDetails = lugar.Tags.Select(t => new
                {
                    t.Id,
                    t.Name
                }).ToList()
            }).FirstOrDefaultAsync();
        if (lugar == null)
        {
            return NotFound();
        }
        return Ok(lugar);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LugaresCreateDto dto)
    {
        if (dto == null)
        {
            Console.WriteLine("Dto é nulooooooooooooo");
        }
        if (string.IsNullOrEmpty(dto.Address))
        {
            Console.WriteLine("Address é nulo ou vazio");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var lugar = new LugarModel
        {
            Name = dto.Name,
            Address = dto.Address ?? "",
            Number = dto.Number ?? "",
            Cep = dto.Cep ?? "",
            CityZone = dto.CityZone,
            HasVisited = dto.HasVisited,
            Avaliation = dto.Avaliation ?? 0,
            Observation = dto.Observation ?? "",
            Tags = await _context.Tags
                .Where(t => dto.TagsIds.Contains(t.Id))
                .ToListAsync()
        };
        _context.Lugares.Add(lugar);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLugarById), new { id = lugar.Id }, lugar);
    }
    
}