namespace NotesWebApp;

public static class CorsConfiguration
{
    private const string DefaultPolicyName = "Default";

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSection = configuration.GetSection("Cors:Policies:Default");
        var origins = corsSection.GetSection("Origins").Get<string[]>() ?? Array.Empty<string>();
        var methods = corsSection.GetSection("Methods").Get<string[]>() ?? new[] { "GET", "POST" };
        var headers = corsSection.GetSection("Headers").Get<string[]>() ?? Array.Empty<string>();
        var allowCredentials = corsSection.GetValue<bool>("AllowCredentials");

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultPolicyName, policy =>
            {
                if (origins.Length > 0)
                {
                    policy.WithOrigins(origins);
                }
                else
                {
                    throw new InvalidOperationException("CORS origins must be configured (Cors:Policies:Default:Origins).");
                }

                policy.WithMethods(methods);
                if (headers.Length > 0) policy.WithHeaders(headers); else policy.DisallowCredentials();

                if (allowCredentials)
                {
                    policy.AllowCredentials();
                }
                else
                {
                    policy.DisallowCredentials();
                }

                policy.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });

        return services;
    }
}
