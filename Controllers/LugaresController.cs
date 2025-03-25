using System.Text.Json;
using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Enums;
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
                CityZoneDetails = new
                {
                    CityZoneId = lugar.CityZone,
                    CityZone = lugar.CityZone.ToString(),
                },                lugar.HasVisited,
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
                lugar.Address,
                lugar.Number,
                lugar.Cep,
                CityZoneDetails = new
                {
                    CityZoneId = lugar.CityZone,
                    CityZone = lugar.CityZone.ToString(),
                },                lugar.HasVisited,
                lugar.Avaliation,
                lugar.Observation,
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

    [HttpGet("zone/{zoneId}")]
    public async Task<IActionResult> GetLugarByZone(int zoneId)
    {
        var lugar = await _context.Lugares
            .Include(lugar => lugar.Tags)
            .Where(lugar => lugar.CityZone == (CityZoneEnum)zoneId)
            .Select(lugar => new
            {
                lugar.Id,
                lugar.Name,
                lugar.Address,
                lugar.Number,
                lugar.Cep,
                CityZoneDetails = new
                {
                    CityZoneId = lugar.CityZone,
                    CityZone = lugar.CityZone.ToString(),
                },                lugar.HasVisited,
                lugar.Avaliation,
                lugar.Observation,
                TagDetails = lugar.Tags.Select(t => new
                {
                    t.Id,
                    t.Name
                }).ToList()
            }).ToListAsync();
        return Ok(lugar);
    }
    
    [HttpGet("visited/{hasVisited}")]
    public async Task<IActionResult> GetLugarByVisited(Boolean hasVisited)
    {
        var lugar = await _context.Lugares
            .Include(lugar => lugar.Tags)
            .Where(lugar => lugar.HasVisited == hasVisited)
            .Select(lugar => new
            {
                lugar.Id,
                lugar.Name,
                lugar.Address,
                lugar.Number,
                lugar.Cep,
                CityZoneDetails = new
                {
                    CityZoneId = lugar.CityZone,
                    CityZone = lugar.CityZone.ToString(),
                },
                lugar.HasVisited,
                lugar.Avaliation,
                lugar.Observation,
                TagDetails = lugar.Tags.Select(t => new
                {
                    t.Id,
                    t.Name
                }).ToList()
            }).ToListAsync();
        return Ok(lugar);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LugaresCreateDto dto)
    {
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

    [HttpPatch]
    public async Task<IActionResult> PatchLugar(int id, [FromBody] LugaresPatchDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var lugar = await _context.Lugares.Include(lugar => lugar.Tags).FirstOrDefaultAsync();
        if (lugar == null)
        {
            return NotFound("Lugar não encontrado");
        }
        if (!string.IsNullOrEmpty(dto.Name))
        {
            lugar.Name = dto.Name;
        }

        if (!string.IsNullOrEmpty(dto.Address))
        {
            lugar.Address = dto.Address;
        }

        if (!string.IsNullOrEmpty(dto.Number))
        {
            lugar.Number = dto.Number;
        }

        if (!string.IsNullOrEmpty(dto.Cep))
        {
            lugar.Cep = dto.Cep;
        }

        if (dto.CityZone.HasValue)
        {
            lugar.CityZone = dto.CityZone.Value;
        }

        if (dto.HasVisited.HasValue)
        {
            lugar.HasVisited = dto.HasVisited.Value;
        }

        if (dto.Avaliation.HasValue)
        {
            lugar.Avaliation = dto.Avaliation.Value;
        }

        if (!string.IsNullOrEmpty(dto.Observation))
        {
            lugar.Observation = dto.Observation;
        }

        if (dto.TagsIds != null && dto.TagsIds.Any())
        {
            lugar.Tags = await _context.Tags.Where(tag => dto.TagsIds.Contains(tag.Id)).ToListAsync();
        }

        _context.Lugares.Update(lugar);
        await _context.SaveChangesAsync();
        return Ok();

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLugar(int id)
    {
        var lugar = await _context.Lugares.FindAsync(id);
        if (lugar == null)
        {
            return NotFound("Lugar não encontrada");
        }

        _context.Lugares.Remove(lugar);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
}