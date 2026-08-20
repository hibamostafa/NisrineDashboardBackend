Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false"); // Fixes the Render startup crash

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

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
                "https://nisrinemasri.vercel.app",       // Your deployed portfolio
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();