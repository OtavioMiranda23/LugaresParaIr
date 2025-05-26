using System.Text.Json;
using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Enums;
using LugaresParaIr.Models;
using LugaresParaIr.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<IActionResult> GetLugares(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var totalRecords = await _context.Lugares.CountAsync();
        var lugares = await _context.Lugares
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(lugar => lugar.Tags)
            .Select(lugar => new LugarDto
            {
                Id = lugar.Id,
                Name = lugar.Name,
                Address = lugar.Address,
                Number = lugar.Number,
                Cep = lugar.Cep,
                CityZoneDetails = new CityZoneDetailsDto
                {
                    CityZoneId = (int?)lugar.CityZone,
                    CityZone = lugar.CityZone.ToString(),
                },                
                HasVisited = lugar.HasVisited,
                Avaliation = lugar.Avaliation,
                Observation = lugar.Observation,
                TagDetails = lugar.Tags.Select(t => new TagDetailsDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            }).ToListAsync();
        var pagedResponse = new PagedResponseOffset<LugarDto>(lugares, pageNumber, pageSize, totalRecords);
        return Ok(lugares);
    }
    
    [Authorize]
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
    
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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
            Users = await _context.User
                .Where(user => dto.UserId == user.Id)
                .ToListAsync(),
            Tags = await _context.Tags
                .Where(t => dto.TagsIds.Contains(t.Id))
                .ToListAsync()
        };
        _context.Lugares.Add(lugar);
        await _context.SaveChangesAsync();
        return Ok(new { id = lugar.Id });
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutLugar(int id, [FromBody] LugaresPatchDto dto)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var lugar = await _context.Lugares.Include(lugar => lugar.Tags).FirstOrDefaultAsync(lugar => lugar.Id == id);
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

        if (dto.TagsIds != null)
        {
            lugar.Tags = await _context.Tags.Where(tag => dto.TagsIds.Contains(tag.Id)).ToListAsync();
        }

        _context.Lugares.Update(lugar);
        await _context.SaveChangesAsync();
        return Ok();

    }
    [Authorize]
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

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetLugaresByUserId(Guid userId)
    {
        var user = await _context.User
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        if (user == null)
            return BadRequest();
        var lugar = await _context.Lugares
            .Include(lugar => lugar.Tags)
            .Where(lugar => lugar.Users.Any(u => u.Id == user.Id))
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
}