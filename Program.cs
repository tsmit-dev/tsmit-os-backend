
using Microsoft.EntityFrameworkCore;
using myapp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using myapp.Services;
using myapp.Models;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.

// 1. Configure Entity Framework Core with PostgreSQL
// This connection string will be provided by Heroku in production
// and from your local environment variables in development.
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DATABASE_URL environment variable not found.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Add Controllers
builder.Services.AddControllers();

// 3. Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configure JWT Authentication using Environment Variables
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new ArgumentNullException("JWT environment variables (JWT_KEY, JWT_ISSUER, JWT_AUDIENCE) must be set.");
}
if (jwtKey.Length < 32)
{
     throw new ArgumentException("JWT_KEY must be at least 32 characters long.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // In production, the "https" part of the URL is handled by the proxy (Heroku), so RequireHttpsMetadata can be false.
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 5. Register Custom Services
// Make sure EncryptionKey is also set as an environment variable
var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
if (string.IsNullOrEmpty(encryptionKey) || encryptionKey.Length < 32)
{
    throw new ArgumentException("ENCRYPTION_KEY environment variable must be set and be at least 32 characters long.");
}

builder.Services.AddSingleton<IDataProtectionService, DataProtectionService>();
builder.Services.AddSingleton<IEmailSettingsService, EmailSettingsService>();
builder.Services.AddScoped<ILogOSService, LogOSService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<ISystemSettingsService, SystemSettingsService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
// Heroku handles HTTPS, so we can conditionally enable Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Heroku's router handles HTTPS termination.
// app.UseHttpsRedirection(); // This can sometimes cause issues with proxies like Heroku. It's safer to disable.

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Heroku provides the PORT environment variable to bind to.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://*:{port}");
}

app.Run();
