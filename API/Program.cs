using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloundinarySettings")); // typo in "CloundinarySettings" is intentional to match appsettings.json
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // to use JWT bearer authentication to check if the user is logged in.
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // If authentication fails, respond with 401 Unauthorized using JWT rules
    })
    .AddJwtBearer(options =>
    {
        // save in httpcontext, usefull if you will use it later in the pipeline to access it directly (HttpContext.GetTokenAsync(...))
        options.SaveToken = true; 
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // to validate that the token wasn't changes (important if there's role claim may changes from user to admin)
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["PrivateKey"] ?? 
                throw new InvalidOperationException("JWT private key is not configured (PrivateKey)."))),
            ValidateIssuer = false, // if true you need to set validIssuer
            ValidateAudience = false // if true you need to set validAudience
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
try
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}
app.Run();
