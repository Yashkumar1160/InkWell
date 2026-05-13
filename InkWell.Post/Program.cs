using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InkWell.Post.Middleware;
using InkWell.Post.Context;
using InkWell.Post.Repository.Interfaces;
using InkWell.Post.Repository.Repositories;
using InkWell.Post.Service.Interfaces;
using MassTransit;


var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;


// Database service
var connUrl = configuration.GetConnectionString("PostDB");
if (connUrl != null && connUrl.Contains("://")) {
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;TrustServerCertificate=True;";
}
builder.Services.AddDbContext<PostDbContext>(options =>
{
    options.UseNpgsql(connUrl);
});


// Redis Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = "InkWellPost_";
});


// RabbitMQ with MassTransit
builder.Services.AddMassTransit(x =>
{
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
    });
});


// Dependency Injection
builder.Services.AddScoped<IPostRepository, PostRepositoryImpl>();
builder.Services.AddScoped<IPostService, InkWell.Post.Service.Services.PostServiceImpl>();


// JWT (same as auth service so that token works accross all services)
byte[] keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]);
var securityKey = new SymmetricSecurityKey(keyBytes);


//Use JWT Bearer as the default method
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


// Authorization
builder.Services.AddAuthorization();


// this registers HttpClient in the DI container

// CORS
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
builder.Services.AddEndpointsApiExplorer();


// Swagger
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

// Build app
var app = builder.Build();

// app.UseExceptionHandler();


// Middlewate Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostDbContext>();
    try { var databaseCreator = db.GetService<IRelationalDatabaseCreator>(); databaseCreator.CreateTables(); } catch { /* Tables already exist or shared DB conflict */ }
}

app.Run();






