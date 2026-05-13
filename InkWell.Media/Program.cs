using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using InkWell.Media.Context;
using InkWell.Media.Repository.Interfaces;
using InkWell.Media.Repository.Repositories;
using InkWell.Media.Services.Interfaces;
using InkWell.Media.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InkWell.Media.Middleware;


var builder = WebApplication.CreateBuilder(args);


// Configuration service
var configuration = builder.Configuration;

// Database service
var connUrl = configuration.GetConnectionString("MediaDB");
if (connUrl != null && connUrl.Contains("://")) {
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;TrustServerCertificate=True;";
}
builder.Services.AddDbContext<MediaDbContext>(options =>
{
    options.UseNpgsql(connUrl);
});

// Dependency Injection
builder.Services.AddScoped<IMediaRepository, MediaRepositoryImpl>();
builder.Services.AddScoped<IMediaService, MediaServiceImpl>();

// jwt - same secret key as all other services
var jwtSecret = configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine("CRITICAL: Jwt:Secret is missing from configuration!");
    jwtSecret = "H7qTSFExjOFBO4w67FN3JbgnVk8YTaNn2Jndvgkqg6I"; 
}
byte[] keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
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

// Controllers
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

var app = builder.Build();

// app.UseExceptionHandler();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

//Serves static files from wwwroot
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
    try { var databaseCreator = db.GetService<IRelationalDatabaseCreator>(); databaseCreator.CreateTables(); } catch { /* Tables already exist or shared DB conflict */ }
}

app.Run();






