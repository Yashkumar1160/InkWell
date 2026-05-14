using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using InkWell.Newsletter.Context;
using InkWell.Newsletter.Repository.Interfaces;
using InkWell.Newsletter.Repository.Repositories;
using InkWell.Newsletter.Services.Interfaces;
using InkWell.Newsletter.Services.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InkWell.Newsletter.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration service
var configuration = builder.Configuration;

// Database context
var connUrl = configuration.GetConnectionString("NewsletterDB");
if (connUrl != null && connUrl.Contains("://")) {
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;TrustServerCertificate=True;";
}
builder.Services.AddDbContext<NewsletterDbContext>(options =>
{
    options.UseNpgsql(connUrl);
});


// RabbitMQ with MassTransit
builder.Services.AddMassTransit(x =>
{
    // Add Consumer
    x.AddConsumer<InkWell.Newsletter.Messaging.Consumers.PostPublishedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitUrl = configuration["RabbitMQ:ConnectionString"] 
                     ?? configuration["RabbitMQ__ConnectionString"] 
                     ?? configuration["RABBITMQ_URL"] 
                     ?? configuration["RabbitMQ:Host"];

        if (!string.IsNullOrEmpty(rabbitUrl) && rabbitUrl.Contains("://"))
        {
            cfg.Host(new Uri(rabbitUrl));
        }
        else
        {
            cfg.Host(rabbitUrl ?? "localhost", "/", h => { });
        }
        cfg.ConfigureEndpoints(context);
    });
});


// Dependency Injection
builder.Services.AddScoped<ISubscriberRepository, SubscriberRepositoryImpl>();
builder.Services.AddScoped<INewsletterService, NewsletterServiceImpl>();

// JWT authentication 
var jwtSecret = configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine("CRITICAL: Jwt:Secret is missing from configuration!");
    jwtSecret = "FPICUVNej03KufTJmHzbToS8jMYHWXIuUkNRGxWHoAg="; 
}
byte[] keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
var securityKey = new SymmetricSecurityKey(keyBytes);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidateLifetime = true
    };
});

// Authorization service
builder.Services.AddAuthorization();

// CORS configuration 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://inkwell-frontend-7ntx.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Controllers
builder.Services.AddControllers(options => 
{
    options.Filters.Add<GlobalExceptionHandler>();
});
// Register Global Exception Handler
// builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// app.UseExceptionHandler();

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NewsletterDbContext>();
    try { var databaseCreator = db.GetService<IRelationalDatabaseCreator>(); databaseCreator.CreateTables(); } catch { /* Tables already exist or shared DB conflict */ }
}

app.Run();






