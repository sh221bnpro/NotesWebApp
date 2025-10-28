using NotesWebApp.Application.Contracts;
using NotesWebApp.Application.DTOs;
using NotesWebApp.Domain.Models;

namespace NotesWebApp.Application;

public class NotesService
{
    private readonly INotesRepository _repository;
    public NotesService(INotesRepository repository) => _repository = repository;

    private static Note Map(NoteDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Content = dto.Content,
        Priority = dto.Priority,
        CreatedUtc = dto.CreatedUtc,
        UpdatedUtc = dto.UpdatedUtc
    };

    private static NoteDto MapDto(Note note) => new(note.Id, note.Title, note.Content, note.Priority, note.CreatedUtc, note.UpdatedUtc);

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        var dtos = await _repository.GetAllAsync().ConfigureAwait(false);
        return dtos.Select(Map)
            .OrderByDescending(n => n.Priority)
            .ThenByDescending(n => n.CreatedUtc);
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        var dto = await _repository.GetByIdAsync(id).ConfigureAwait(false);
        return dto is null ? null : Map(dto);
    }

    public async Task<Note> AddAsync(Note note)
    {
        var dto = await _repository.AddAsync(MapDto(note)).ConfigureAwait(false);
        return Map(dto);
    }

    public async Task UpdateAsync(Note note)
    {
        await _repository.UpdateAsync(MapDto(note)).ConfigureAwait(false);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
}
