using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using InkWell.Comment.Context;
using InkWell.Comment.Repository.Interfaces;
using InkWell.Comment.Repository.Repositories;
using InkWell.Comment.Services.Interfaces;
using InkWell.Comment.Services.Services;
using InkWell.Comment.Messaging.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InkWell.Comment.Middleware;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// database
var connUrl = configuration.GetConnectionString("CommentDB");
if (connUrl != null && connUrl.Contains("://")) {
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;TrustServerCertificate=True;";
}
builder.Services.AddDbContext<CommentDbContext>(options =>
{
    options.UseNpgsql(connUrl);
});


// Redis Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = "InkWellComment_";
});


// RabbitMQ with MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PostDeletedConsumer>();

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
        
        // Define the queue that listens for deleted posts
        cfg.ReceiveEndpoint("comment-post-deleted-queue", e => {
            e.ConfigureConsumer<PostDeletedConsumer>(context);
        });
    });
});


// dependency injection
builder.Services.AddScoped<ICommentRepository, CommentRepositoryImpl>();
builder.Services.AddScoped<ICommentService, CommentServiceImpl>();

// jwt - same secret key as all other services
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

builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://inkwell-frontend-qv2r.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options => 
{
    options.Filters.Add<GlobalExceptionHandler>();
});
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

var app = builder.Build();

// app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    var databaseCreator = db.GetService<IRelationalDatabaseCreator>();
    if (!databaseCreator.Exists())
    {
        databaseCreator.Create();
    }
    try { databaseCreator.CreateTables(); } catch { /* Tables already exist or shared DB conflict */ }
}

app.Run();






