using System.Text;
using InkWell.Auth.Context;
using InkWell.Auth.Repository.Interface;
using InkWell.Auth.Repository.Repositories;
using InkWell.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Database Setup
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("AuthDB"));
});


// Dependency Injection (One instance per HTTP request)
builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IAuthService, AuthService.Services.Service.AuthServiceImpl>();


// JWT authentication 
// get secret key from appsettings.json and convert to bytes
byte[] keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]);
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
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Controllers
builder.Services.AddControllers();

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
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

