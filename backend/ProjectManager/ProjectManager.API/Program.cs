using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;

var builder = WebApplication.CreateBuilder(args);

// Configure Database & Identity Systems
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register AppDbContext with SQLite provider
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connectionString));

builder.Services.AddAuthentication();

// Register ASP.NET Core Identity Core services
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<AppDbContext>();

var builderControllers = builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManagerActual = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await ProjectManager.DAL.Seeding.DatabaseSeeder.SeedAsync(userManager, roleManagerActual);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
