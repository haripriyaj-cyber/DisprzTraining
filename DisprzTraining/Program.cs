using DisprzTraining.Utils;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DisprzTraining.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode;
    });

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "DisprzTraining API",
        Version = "v1",
        Description = "API for managing appointments"
    });
});

// Configure dependency injection
builder.Services.ConfigureDependencyInjections();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DisprzTraining API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
    
    // Skip HTTPS redirection in development
    // app.UseHttpsRedirection(); -- Comment this out
}
else
{
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
