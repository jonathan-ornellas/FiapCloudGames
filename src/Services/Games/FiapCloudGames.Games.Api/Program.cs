using FiapCloudGames.Games.Api.Data;
using FiapCloudGames.Games.Api.Repositories;
using FiapCloudGames.Shared.Elasticsearch;
using FiapCloudGames.Shared.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Serilog;
using System.Text;
using FiapCloudGames.Games.Business;
using FiapCloudGames.Domain;
using FluentValidation.AspNetCore;
using FluentValidation;
using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<FiapCloudGames.Games.Api.Validators.CreateGameDtoValidator>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Games API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<GamesContext>(options =>
    options.UseSqlServer(connectionString));

var elasticsearchUrl = builder.Configuration.GetValue<string>("Elasticsearch:Url")
    ?? Environment.GetEnvironmentVariable("ELASTICSEARCH_URL") 
    ?? "http://localhost:9200";

var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("games");

builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var cfg = builder.Configuration;
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Jwt:Issuer"],
            ValidAudience = cfg["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost";
var rabbitMqUsername = builder.Configuration["RabbitMq:Username"] ?? Environment.GetEnvironmentVariable("RabbitMq__Username") ?? "guest";
var rabbitMqPassword = builder.Configuration["RabbitMq:Password"] ?? Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "guest";

builder.Services.AddSingleton<IRabbitMQConsumer>(sp => new RabbitMQConsumer(rabbitMqHost, rabbitMqUsername, rabbitMqPassword));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCustomExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    var runMigrations = builder.Configuration.GetValue<bool>("RunMigrations", false);
    if (runMigrations)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GamesContext>();
            db.Database.Migrate();
        }
    }
}

app.Run();


public partial class Program { }
