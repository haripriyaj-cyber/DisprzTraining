using DisprzTraining.Utils;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DisprzTraining.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// ✅ Use both HTTP and HTTPS ports for development
builder.WebHost.UseUrls("http://localhost:5002", "https://localhost:5003");

// Add JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.UnknownTypeHandling = 
            System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode;
    });

// Add database context
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

// ✅ Configure CORS to allow React frontend with both HTTP and HTTPS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure dependency injections
builder.Services.ConfigureDependencyInjections();

var app = builder.Build();

// ✅ Enable CORS before authorization
app.UseCors();

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DisprzTraining API v1");
        c.RoutePrefix = string.Empty;
    });
}

// ✅ Redirect HTTP to HTTPS (optional in dev, can skip if using only HTTPS port)
// You can comment this out if you want to use HTTP directly

// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
