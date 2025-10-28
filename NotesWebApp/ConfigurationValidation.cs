namespace NotesWebApp;

public static class ConfigurationValidation
{
    public static void ValidateRequiredConfiguration(this IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new NullReferenceException($"Missing required configuration value '{key}'.");
        }
    }
}