using System.ComponentModel.DataAnnotations;

namespace NotesWebApp.Application.DTOs;

public record NoteDto(int Id, string Title, string Content, int Priority, DateTime CreatedUtc, DateTime? UpdatedUtc);

public record CreateNoteRequest(
    [Required, MaxLength(200)] string Title,
    [Required, MaxLength(4000)] string Content,
    [Range(0, 10)] int Priority);

public class UpdateNoteRequest
{
    [Required]
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(4000)]
    public string Content { get; set; } = string.Empty;
    [Range(0, 10)]
    public int Priority { get; set; }

    public UpdateNoteRequest() { }
}
