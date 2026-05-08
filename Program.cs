using Microsoft.EntityFrameworkCore;
using MyPortfolioBackend.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Get Connection String from Environment (Railway/Render will provide this)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. DYNAMIC CORS (Allow everything for now so it works on Vercel immediately)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CloudPolicy", policy => 
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable Swagger in Cloud so you can see it's working
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("CloudPolicy");
app.UseAuthorization();
app.MapControllers();

// 3. LISTEN TO PORT (Required for Cloud hosting)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");