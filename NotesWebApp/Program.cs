using NotesWebApp;
using NotesWebApp.Application;
using NotesWebApp.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
 .AddEnvironmentVariables();

builder.Configuration.ValidateRequiredConfiguration("ConnectionStrings:DefaultConnection");

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("api", context =>
    RateLimitPartition.GetTokenBucketLimiter(partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host,
    factory: key => new TokenBucketRateLimiterOptions
    {
        TokenLimit = 100,
        TokensPerPeriod = 50,
        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
        AutoReplenishment = true,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0
    }));
});

builder.Services.AddHttpsRedirection(o => { o.HttpsPort = 443; });

builder.Services.AddHsts(o =>
{
    o.Preload = true;
    o.IncludeSubDomains = true;
    o.MaxAge = TimeSpan.FromDays(365);
});

builder.Services
 .AddConfiguredCors(builder.Configuration)
 .AddPersistence(builder.Configuration)
 .AddApplication()
 .AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddScoped<NotesService>();

builder.Services.AddAntiforgery(o => { o.Cookie.SecurePolicy = CookieSecurePolicy.Always; });

builder.Services.Configure<CookiePolicyOptions>(o => { o.Secure = CookieSecurePolicy.Always; });

var app = builder.Build();

await app.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
    app.UseExceptionHandler("/Home/Error");
    app.UseGlobalExceptionHandling();
}

app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("Default");

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapGet("/", () => Results.Redirect("/Notes"));

app.Run();
