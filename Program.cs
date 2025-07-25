
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

// 1. Add the AWS Lambda hosting services. This is the key change for Netlify.
// THIS IS THE CORRECTED LINE: Called on builder.Services
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// 2. Configure Entity Framework Core with PostgreSQL
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(connectionString))
{
    // In a serverless environment, this should always be provided.
    // Throw an exception during build/startup if it's not found.
    throw new InvalidOperationException("DATABASE_URL environment variable not found. This is required to connect to the database.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Add Controllers
builder.Services.AddControllers();

// 4. Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Configure JWT Authentication using Environment Variables
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

// 6. Register Custom Services
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
// Swagger is useful for local testing, but not typically exposed in a production serverless function.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// app.Run() is replaced by the Lambda hosting services.
app.Run();
