using Microsoft.EntityFrameworkCore;
using NotesWebApp.Application;
using NotesWebApp.Application.Contracts;
using NotesWebApp.Persistence;

namespace NotesWebApp;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<NotesService>();
        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));

        services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.CommandTimeout(30);
            });
        });

        services.AddScoped<INotesRepository, NotesRepository>();
        return services;
    }
}
