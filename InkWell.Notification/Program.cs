using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using InkWell.Notification.Context;
using InkWell.Notification.Repository.Interfaces;
using InkWell.Notification.Repository.Repositories;
using InkWell.Notification.Services.Interfaces;
using InkWell.Notification.Services.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InkWell.Notification.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration service
var configuration = builder.Configuration;

// Database service
var connUrl = configuration.GetConnectionString("NotificationDB");
if (connUrl != null && connUrl.Contains("://")) {
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;TrustServerCertificate=True;";
}
builder.Services.AddDbContext<NotificationDbContext>(options =>
{
    options.UseNpgsql(connUrl);
});


// RabbitMQ with MassTransit
builder.Services.AddMassTransit(x =>
{
    // Add Consumers
    x.AddConsumer<InkWell.Notification.Messaging.Consumers.PostLikedConsumer>();
    x.AddConsumer<InkWell.Notification.Messaging.Consumers.CommentAddedConsumer>();
    x.AddConsumer<InkWell.Notification.Messaging.Consumers.PostPublishedConsumer>();
    x.AddConsumer<InkWell.Notification.Messaging.Consumers.NewsletterPublishedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // Try multiple possible keys to be extra safe
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
            // Default to localhost only if no cloud URL is found
            cfg.Host(rabbitUrl ?? "localhost", "/", h => { });
        }
        cfg.ConfigureEndpoints(context);
    });
});


// Dependency Injection
builder.Services.AddScoped<INotificationRepository, NotificationRepositoryImpl>();
builder.Services.AddScoped<INotificationService, NotificationServiceImpl>();

// JWT Authentication
// JWT (same secret as Auth for verification)
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

// Authorization Service
builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://inkwell-frontend-qv2r.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Controllers
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
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    try { var databaseCreator = db.GetService<IRelationalDatabaseCreator>(); databaseCreator.CreateTables(); } catch { /* Tables already exist or shared DB conflict */ }
}

app.Run();






