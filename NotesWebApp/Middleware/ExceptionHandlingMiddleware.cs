using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Diagnostics;

namespace NotesWebApp.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next; _logger = logger; _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var isApi = context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ||
        context.Request.Headers.Accept.Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var cid)
        ? cid.ToString()
        : Guid.NewGuid().ToString("N");
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        _logger.LogError(ex,
        "Unhandled exception. CorrelationId={CorrelationId} TraceId={TraceId} Path={Path} Method={Method}",
        correlationId, traceId, context.Request.Path, context.Request.Method);

        context.Response.Clear();
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        if (isApi)
        {
            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = context.Response.StatusCode,
                Detail = _env.IsDevelopment() ? ex.ToString() : ex.Message,
                Instance = context.Request.Path
            };
            problem.Extensions["correlationId"] = correlationId;
            problem.Extensions["traceId"] = traceId;
            if (!_env.IsDevelopment())
            {
                //suppress
            }
            var json = JsonSerializer.Serialize(problem);
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }
        else
        {
            context.Response.Redirect("/Home/Error?cid=" + correlationId);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
