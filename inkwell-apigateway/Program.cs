using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
var ocelotFile = builder.Environment.EnvironmentName == "Docker"
    ? "ocelot.Docker.json"
    : "ocelot.json";

builder.Configuration.AddJsonFile(ocelotFile, optional: false, reloadOnChange: true);

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);

// Add Swagger services for Gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "InkWell Gateway API", Version = "v1" });
});

// Add CORS - allow Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:4200", "http://localhost:4201")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
    // Fallback for dev
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add routing with case-insensitive matching
builder.Services.AddRouting(options => options.LowercaseUrls = false);

var app = builder.Build();

// CORS must be FIRST
app.UseCors("AllowAll");

// Health Check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

// Swagger UI (Must be before Ocelot)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API");
    c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service");
    c.SwaggerEndpoint("/post/swagger/v1/swagger.json", "Post Service");
    c.SwaggerEndpoint("/comment/swagger/v1/swagger.json", "Comment Service");
    c.SwaggerEndpoint("/category/swagger/v1/swagger.json", "Category Service");
    c.SwaggerEndpoint("/media/swagger/v1/swagger.json", "Media Service");
    c.SwaggerEndpoint("/newsletter/swagger/v1/swagger.json", "Newsletter Service");
    c.SwaggerEndpoint("/notification/swagger/v1/swagger.json", "Notification Service");
    c.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));

// Ocelot MUST be LAST
await app.UseOcelot();

app.Run();
