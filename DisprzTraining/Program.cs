using DisprzTraining.Utils;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DisprzTraining.DataAccess;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

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

// Configure Swagger with enhanced documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Documentation",
        Version = "v1",
        Description = "API Documentation"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Organize endpoints by controller
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((docName, api) => true);
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

// Enable Swagger with improved UI
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); // Remove the options parameter
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Appointment Scheduler API v1");
        c.RoutePrefix = string.Empty; // Serve the Swagger UI at the application's root
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DefaultModelsExpandDepth(1);
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

// app.UseHttpsRedirection(); // Uncomment if you want to force HTTPS

app.UseAuthorization();
app.MapControllers();

app.Run();
