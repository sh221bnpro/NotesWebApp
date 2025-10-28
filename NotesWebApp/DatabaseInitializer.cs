using Microsoft.EntityFrameworkCore;
using NotesWebApp.Persistence;
using NotesWebApp.Persistence.Entities;

namespace NotesWebApp;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var db = await factory.CreateDbContextAsync();

        try
        {
            logger.LogInformation("Provider: {Provider}", db.Database.ProviderName);
            var applied = await db.Database.GetAppliedMigrationsAsync();
            var pending = await db.Database.GetPendingMigrationsAsync();

            if (!applied.Any())
            {
                await db.Database.EnsureCreatedAsync();
            }
            else if (pending.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
                await db.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("No pending migrations; schema up to date.");
            }

            if (!await db.Notes.AnyAsync())
            {
                logger.LogInformation("Seeding initial notes...");
                db.Notes.AddRange(
                new NoteEntity { Title = "Welcome to Scribbler", Content = "This is your first note. Feel free to edit or delete it.", Priority = 1, CreatedUtc = DateTime.UtcNow },
                new NoteEntity { Title = "Getting Started", Content = "Create a new note using the New Note button.", Priority = 2, CreatedUtc = DateTime.UtcNow }
                );
                await db.SaveChangesAsync();
                logger.LogInformation("Seed data inserted.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed.");
            if (env.IsDevelopment()) throw;
        }
    }
}
