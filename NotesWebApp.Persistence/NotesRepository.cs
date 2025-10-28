using Microsoft.EntityFrameworkCore;
using NotesWebApp.Application.Contracts;
using NotesWebApp.Application.DTOs;
using NotesWebApp.Persistence.Entities;

namespace NotesWebApp.Persistence;

public class NotesRepository : INotesRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    public NotesRepository(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<IEnumerable<NoteDto>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await context.Notes.AsNoTracking()
            .Select(n => new NoteDto(n.Id, n.Title, n.Content, n.Priority, n.CreatedUtc, n.UpdatedUtc))
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<NoteDto?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await context.Notes.AsNoTracking()
            .Where(n => n.Id == id)
            .Select(n => new NoteDto(n.Id, n.Title, n.Content, n.Priority, n.CreatedUtc, n.UpdatedUtc))
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<NoteDto> AddAsync(NoteDto note)
    {
        await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var entity = new NoteEntity
        {
            Title = note.Title,
            Content = note.Content,
            Priority = note.Priority,
            CreatedUtc = note.CreatedUtc,
            UpdatedUtc = note.UpdatedUtc
        };
        context.Notes.Add(entity);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return new NoteDto(entity.Id, entity.Title, entity.Content, entity.Priority, entity.CreatedUtc, entity.UpdatedUtc);
    }

    public async Task UpdateAsync(NoteDto note)
    {
        await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var entity = await context.Notes.FindAsync(new object?[] { note.Id }).ConfigureAwait(false);
        if (entity is null) return;
        entity.Title = note.Title;
        entity.Content = note.Content;
        entity.Priority = note.Priority;
        entity.UpdatedUtc = note.UpdatedUtc ?? DateTime.UtcNow;
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var entity = await context.Notes.FindAsync(new object?[] { id }).ConfigureAwait(false);
        if (entity is null) return;
        context.Notes.Remove(entity);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
