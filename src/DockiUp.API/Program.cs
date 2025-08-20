using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using DockiUp.Application.Queries;
using DockiUp.Application.Validators;
using DockiUp.Infrastructure;
using DockiUp.Infrastructure.Clients;
using DockiUp.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register Swagger services
builder.Services.AddSwaggerGen();

// Add Services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAppInfoQuery).Assembly));
builder.Services.Configure<SystemPaths>(builder.Configuration.GetSection("SystemPaths"));

builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<IDockiUpProjectConfigurationService, DockiUpProjectConfigurationService>();

builder.Services.AddSingleton<IDockiUpDockerClient, DockiUpDockerClient>();

// Add Validators
builder.Services.AddValidatorsFromAssemblyContaining<DeployProjectCommandValidator>();

// Configure the DbContext with a connection string.
builder.Services.AddDbContext<DockiUpDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DockiUpDatabase"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null)
    ));

builder.Services.AddScoped<IDockiUpDbContext>(provider =>
        provider.GetRequiredService<DockiUpDbContext>()
    );

// Add CORS policy to allow all origins, methods, and headers
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(corsBuilder =>
        {
            corsBuilder
                .WithOrigins("http://localhost:4200")
                .WithExposedHeaders("Content-Disposition")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DockiUpDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DockiUp API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Ensure frontend routes work
app.UseRouting();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();