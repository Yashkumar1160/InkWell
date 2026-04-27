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


var builder = WebApplication.CreateBuilder(args);


// Configuration service
var configuration = builder.Configuration;

// Database service
builder.Services.AddDbContext<MediaDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("MediaDB"));
});

// Dependency Injection
builder.Services.AddScoped<IMediaRepository, MediaRepositoryImpl>();
builder.Services.AddScoped<IMediaService, MediaServiceImpl>();

// jwt - same secret key as all other services
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


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Controllers
builder.Services.AddControllers();
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
    db.Database.Migrate();
}

app.Run();