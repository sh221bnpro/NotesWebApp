using Microsoft.EntityFrameworkCore;
using NotesWebApp.Persistence.Entities;

namespace NotesWebApp.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NoteEntity> Notes => Set<NoteEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var notes = modelBuilder.Entity<NoteEntity>();
        notes.ToTable("Notes");
        notes.HasKey(n => n.Id);
        notes.Property(n => n.Title).IsRequired().HasMaxLength(200);
        notes.Property(n => n.Content).IsRequired().HasMaxLength(4000);
        notes.Property(n => n.Priority).HasDefaultValue(0);
        notes.Property(n => n.CreatedUtc).IsRequired();

        // Seed data
        notes.HasData(
        new NoteEntity
        {
            Id = 1,
            Title = "Welcome to Scribbler",
            Content = "This is your first note. Feel free to edit or delete it.",
            Priority = 1,
            CreatedUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new NoteEntity
        {
            Id = 2,
            Title = "Getting Started",
            Content = "Create a new note using the New Note button.",
            Priority = 2,
            CreatedUtc = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        }
        );
    }
}
