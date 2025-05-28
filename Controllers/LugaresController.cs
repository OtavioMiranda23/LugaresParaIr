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
    // [Authorize]
    public async Task<IActionResult> GetLugares(
        int pageNumber = 1, 
        int pageSize = 10,
        Guid? userId = null,
        string? name = null,
        string? addresss = null,
        string? cep = null,
        int? cityZone = null,
        bool? hasVisited = null,
        int? avaliation = null,
        int? tagId = null,
        DateTime? createdAt = null
        )
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var query = _context.Lugares
            .AsQueryable();

        if (name != null)
        {
            query = query.Where(l => l.Name == name);
        }

        if (addresss != null)
        {
            query = query.Where(l => l.Address != null && l.Address.Contains(addresss));
        }

        if (cep != null)
        {
            query = query.Where(l => l.Cep != null && l.Cep == cep);
        }
        
        if (cityZone != null)
        {
            query = query.Where(l => l.CityZone != null && (int)l.CityZone.Value == cityZone);
        }
        if (hasVisited != null)
        {
            query = query.Where(l => l.HasVisited != null && l.HasVisited == hasVisited);
        }
        if (avaliation != null)
        {
            query = query.Where(l => l.Avaliation != null && l.Avaliation == avaliation);
        }
        if (tagId != null)
        {
            query = query.Where(l => l.Tags.Any(t => t.Id == tagId));
        }
        if (userId.HasValue)
        {
            query = query.Where(l => l.Users.Any(u => u.Id == userId.Value));
        }

        query = query.Include(l => l.Tags);

        var totalRecords = await query.CountAsync();
        var lugares = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
        return Ok(pagedResponse);
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
}