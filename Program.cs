Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");

using Microsoft.EntityFrameworkCore;
using MyPortfolioBackend.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Get Connection String from Environment (Ensure "ConnectionStrings:DefaultConnection" is set on Render's Environment Variables!)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("https://nisrine-dashboard.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// CRITICAL: Custom middleware that guarantees CORS headers are preserved even if the application crashes (500 Error)
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var headers = context.Response.Headers;
        if (!headers.ContainsKey("Access-Control-Allow-Origin"))
        {
            headers["Access-Control-Allow-Origin"] = "https://nisrine-dashboard.vercel.app";
            headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
            headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
        }
        return Task.CompletedTask;
    });
    await next();
});

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("Frontend");

app.UseAuthorization();
app.MapControllers();

// 3. LISTEN TO PORT (Required for Cloud hosting)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");