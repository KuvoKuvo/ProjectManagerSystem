using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.Employee;
using ProjectManager.BLL.Services.Project;
using ProjectManager.BLL.Services.Task;
using ProjectManager.API.Services;

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

// This scans the BLL assembly and automatically registers our MappingProfile
builder.Services.AddAutoMapper(typeof(ProjectManager.BLL.Mapping.MappingProfile));

// Scoped lifetime means a new instance is created per each HTTP request from Vue.js
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddScoped<ILocalFileService, LocalFileService>();


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
