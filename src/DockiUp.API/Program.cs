using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using DockiUp.Application.Queries;
using DockiUp.Infrastructure.Clients;
using DockiUp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register Swagger services
builder.Services.AddSwaggerGen();

// Add Services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAppInfoQuery).Assembly));
builder.Services.Configure<SystemPaths>(builder.Configuration.GetSection("SystemPaths"));

builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<IDockiUpProjectConfigurationService, DockiUpProjectConfigurationService>();

builder.Services.AddSingleton<IDockiUpDockerClient, DockiUpDockerClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
