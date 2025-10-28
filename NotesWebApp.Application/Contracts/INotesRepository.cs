using NotesWebApp.Application.DTOs;

namespace NotesWebApp.Application.Contracts;
public interface INotesRepository
{
    Task<IEnumerable<NoteDto>> GetAllAsync();
    Task<NoteDto?> GetByIdAsync(int id);
    Task<NoteDto> AddAsync(NoteDto note);
    Task UpdateAsync(NoteDto note);
    Task DeleteAsync(int id);
}
