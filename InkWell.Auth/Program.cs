using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using InkWell.Auth.Context;
using InkWell.Auth.Repository.Interface;
using InkWell.Auth.Repository.Repositories;
using InkWell.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InkWell.Auth.Middleware;
using InkWell.Auth.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Database Setup
var connUrl = configuration.GetConnectionString("AuthDB");
if (connUrl != null && connUrl.Contains("://")) {
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;TrustServerCertificate=True;";
}
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(connUrl);
});


// Dependency Injection (One instance per HTTP request)
builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IAuthService, AuthService.Services.Service.AuthServiceImpl>();


// JWT authentication 
var jwtSecret = configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine("CRITICAL: Jwt:Secret is missing from configuration!");
    jwtSecret = "FPICUVNej03KufTJmHzbToS8jMYHWXIuUkNRGxWHoAg="; 
}
byte[] keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
var securityKey = new SymmetricSecurityKey(keyBytes);


//Use JWT Bearer as the default method
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Check these rules on every request
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // check that token was signed with our secret key 
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,

        // check that token was meant for InkWell Users
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],

        ValidateAudience=true,
        ValidAudience=configuration["Jwt:Audience"],

        // check token expiry
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();


// CORS Setup (allows angular to call this api)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        // Only allow requests from angular server
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
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

// Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    try { var databaseCreator = db.GetService<IRelationalDatabaseCreator>(); databaseCreator.CreateTables(); } catch { /* Tables already exist or shared DB conflict */ }
}

// Auto-create Admin User
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    var adminEmail = "admin@gmail.com";
    var adminUsername = "admin_super"; 
    
    // Check if either the email or username is already taken
    var existingByEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
    var existingByUsername = await context.Users.FirstOrDefaultAsync(u => u.Username == adminUsername);

    if (existingByEmail == null && existingByUsername == null)
    {
        var newAdmin = new User
        {
            Username = adminUsername,
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin1234"),
            FullName = "Administrator",
            Role = "ADMIN",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Provider = "LOCAL"
        };
        context.Users.Add(newAdmin);
        await context.SaveChangesAsync();
    }
    else if (existingByEmail != null && existingByEmail.Role != "ADMIN")
    {
        existingByEmail.Role = "ADMIN";
        await context.SaveChangesAsync();
    }
}

app.Run();





