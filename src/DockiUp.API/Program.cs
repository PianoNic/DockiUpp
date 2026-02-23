using DockiUp.API.SignalR;
using DockiUp.Application;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using DockiUp.Infrastructure;
using DockiUp.Infrastructure.Clients;
using DockiUp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

builder.Services.AddApplicationModule();

builder.Services.Configure<SystemPaths>(builder.Configuration.GetSection("SystemPaths"));
builder.Services.Configure<DockiUp.Application.Models.DockiUpWebhookOptions>(builder.Configuration.GetSection("Webhook"));
builder.Services.Configure<DockiUp.API.HostedServices.PeriodicUpdateOptions>(options =>
{
    options.PollInterval = TimeSpan.FromMinutes(1);
});
builder.Services.AddHostedService<DockiUp.API.HostedServices.PeriodicUpdateHostedService>();
builder.Services.AddHostedService<DockiUp.API.HostedServices.ContainerStateBroadcastHostedService>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IDockiUpHubBroadcastService, DockiUpHubBroadcastService>();

builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<IDockiUpProjectConfigurationService, DockiUpProjectConfigurationService>();
builder.Services.AddSingleton<IDockiUpDockerClient, DockiUpDockerClient>();
#endregion

#region Database Configuration
builder.Services.AddDbContext<DockiUpDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DockiUpDatabase"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null)));

builder.Services.AddScoped<IDockiUpDbContext>(provider =>
    provider.GetRequiredService<DockiUpDbContext>());
#endregion

#region CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        var allowedOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? new[] { "http://localhost:4200" };

        corsBuilder.WithOrigins(allowedOrigins);
        corsBuilder.WithExposedHeaders("Content-Disposition");
        corsBuilder.AllowAnyHeader();
        corsBuilder.AllowAnyMethod();
        corsBuilder.AllowCredentials();
    });
});
#endregion

var app = builder.Build();

if (app.Environment.IsProduction())
{
    var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
    if (string.IsNullOrWhiteSpace(corsOrigins) || corsOrigins.Trim() == "http://localhost:4200")
        app.Services.GetRequiredService<ILogger<Program>>().LogWarning("CORS may be using default origin in production. Set CORS_ALLOWED_ORIGINS.");
}

#region Database Initialization with Retry Logic
bool dbConnected = false;
int retryCount = 0;
const int maxRetries = 10;
const int retryDelaySeconds = 5;
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();

while (!dbConnected && retryCount < maxRetries)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DockiUpDbContext>();
        startupLogger.LogInformation("Attempting database connection and migrations (Attempt {Attempt}/{MaxRetries})...", retryCount + 1, maxRetries);
        dbContext.Database.Migrate();
        dbConnected = true;
        startupLogger.LogInformation("Database connection successful and migrations applied.");
    }
    catch (NpgsqlException ex)
    {
        startupLogger.LogError(ex, "Database connection failed: {ErrorMessage}", ex.Message);
        retryCount++;
        if (retryCount < maxRetries)
        {
            startupLogger.LogInformation("Retrying in {Delay} seconds...", retryDelaySeconds);
            Thread.Sleep(TimeSpan.FromSeconds(retryDelaySeconds));
        }
        else
        {
            startupLogger.LogCritical("Failed to connect to the database after {MaxRetries} retries. Application will terminate.", maxRetries);
            throw;
        }
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex, "Unexpected error during database setup: {ErrorMessage}", ex.Message);
        retryCount++;
        if (retryCount < maxRetries)
        {
            startupLogger.LogInformation("Retrying in {Delay} seconds...", retryDelaySeconds);
            Thread.Sleep(TimeSpan.FromSeconds(retryDelaySeconds));
        }
        else
        {
            startupLogger.LogCritical("Database operations failed after {MaxRetries} retries.", maxRetries);
            throw;
        }
    }
}
#endregion

#region Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapHub<DockiUpHub>("/hubs/dockiup");

app.MapFallbackToFile("index.html");
#endregion

app.Run();
