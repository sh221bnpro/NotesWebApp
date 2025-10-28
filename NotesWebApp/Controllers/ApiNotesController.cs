using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotesWebApp.Application;
using NotesWebApp.Application.DTOs;
using NotesWebApp.Domain.Models;

namespace NotesWebApp.Controllers;

[ApiController]
[Route("api/Notes")]
[EnableRateLimiting("api")]
public class ApiNotesController : ControllerBase
{
    private readonly NotesService _service;
    public ApiNotesController(NotesService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        var notes = await _service.GetAllAsync();
        return Ok(notes.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> Get(int id)
    {
        var note = await _service.GetByIdAsync(id);
        return note == null ? NotFound() : Ok(MapToDto(note));
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            Priority = request.Priority,
            CreatedUtc = DateTime.UtcNow
        };
        var created = await _service.AddAsync(note);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, MapToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteRequest request)
    {
        if (id != request.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.Title = request.Title;
        existing.Content = request.Content;
        existing.Priority = request.Priority;
        existing.UpdatedUtc = DateTime.UtcNow;
        await _service.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _service.DeleteAsync(id);
        return NoContent();
    }

    private static NoteDto MapToDto(Note n) => new(n.Id, n.Title, n.Content, n.Priority, n.CreatedUtc, n.UpdatedUtc);
}
