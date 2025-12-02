using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// this means that you can get the values of cloudinary settings from appsettings.json 
// file when you inject IOptions<CloudinarySettings>
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloundinarySettings"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<LogUserActivity>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<AppDbContext>();
 
builder.Services.AddAuthentication(options => {
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // 1. Grab the token from the query string
                var accessToken = context.Request.Query["access_token"];

                // 2. Check the request path
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken; // Attach token to context (context of the request not the hub context)
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator")); // the user must be either in Admin or Moderator role
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();
// Simple CORS registration - configure policy inline in the pipeline
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

// CORS: allow Angular dev server on 4200
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles(); // Looks inside wwwroot/ and finds index.html file and serves it
app.UseStaticFiles(); // Allows .NET to serve js, css, images, etc

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/messages");
app.MapFallbackToController("Index", "Fallback"); // if the user navigates to a route that doesn't exist, redirect to the index page

using var scope = app.Services.CreateScope();
try
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await context.Connections.ExecuteDeleteAsync();
    await context.Groups.ExecuteDeleteAsync();
    await Seed.SeedUsers(userManager);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}
app.Run();
