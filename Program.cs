using Microsoft.EntityFrameworkCore;
using MyPortfolioBackend.Data;

Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false"); // Fixes the Render startup crash

var builder = WebApplication.CreateBuilder(args);

// Render exposes the port through the PORT environment variable. Bind to it
// explicitly so the service is reachable by Render's health checks.
if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Swapped UseSqlServer -> UseNpgsql for PostgreSQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                "https://nisrine-dashboard.vercel.app", // Your deployed dashboard
                "https://nisrine-masri-five.vercel.app",       // Your deployed portfolio
                "http://localhost:5173"                  // Keep local Vite/React working!
              )
              .AllowAnyHeader()  // Allows JSON headers (Content-Type) and authorization tokens
              .AllowAnyMethod(); // Allows GET, POST, PUT, DELETE, and OPTIONS requests
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Correct middleware sequence
app.UseRouting();

// Activate the "Frontend" policy
app.UseCors("Frontend");

// TLS is terminated by Render's reverse proxy in production.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();