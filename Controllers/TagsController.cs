using LugaresParaIr.Data;
using LugaresParaIr.Dtos;
using LugaresParaIr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace LugaresParaIr.Controllers;

[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetTasks()
    {
        return await _context.Tags.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        return Ok(tag);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagCreateDto tagDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tag = new TagModel { Name = tagDto.Name };
        _context.Add(tag);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] TagCreateDto tagDto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound("Tag não encontrada");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        tag.Name = tagDto.Name;
        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, "Erro ao atualizar a tag");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound("Tag não encontrada");
        }
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}