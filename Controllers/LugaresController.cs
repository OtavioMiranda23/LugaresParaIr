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
    public async Task<ActionResult<IEnumerable<LugarModel>>> GetLugares()
    {
        return await _context.Lugares.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLugarById(int id)
    {
        var lugar = await _context.Lugares
            .Where(l => l.Id == id)
            .Include(l => l.Tags)
            .Select(l => new
            {
                l.Id,
                l.Name,
                TagDetails = l.Tags.Select(t => new
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var lugar = new LugarModel
        {
            Name = dto.Name,
            Address = dto.Address,
            Number = dto.Number,
            Cep = dto.Cep,
            CityZone = dto.CityZone,
            HasVisited = dto.HasVisited,
            Avaliation = dto.Avaliation,
            Observation = dto.Observation,
            Tags = await _context.Tags
                .Where(t => dto.TagsIds.Contains(t.Id))
                .ToListAsync()
        };
        _context.Lugares.Add(lugar);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLugarById), new { id = lugar.Id }, lugar);
    }
    
}