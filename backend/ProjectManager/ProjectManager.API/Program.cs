using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.Employee;
using ProjectManager.BLL.Services.File;
using ProjectManager.BLL.Services.Project;
using ProjectManager.BLL.Services.Task;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Database & Identity Systems
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register AppDbContext with SQLite provider
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ProjectManager.DAL.UnitOfWork.IUnitOfWork, ProjectManager.DAL.UnitOfWork.UnitOfWork>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
})
.AddIdentityCookies();

// Register ASP.NET Core Identity Core services
builder.Services.AddIdentityCore<Employee>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<ApplicationRole>()
.AddSignInManager()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = "ProjectManager.Auth";

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return System.Threading.Tasks.Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return System.Threading.Tasks.Task.CompletedTask;
    };
});

// This scans the BLL assembly and automatically registers our MappingProfile
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(ProjectManager.BLL.Mapping.MappingProfile).Assembly);
});

// Scoped lifetime means a new instance is created per each HTTP request from Vue.js
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddScoped<ILocalFileService, LocalFileService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddOpenApi();

var builderControllers = builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("VueCorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        var userManager = services.GetRequiredService<UserManager<Employee>>();
        var roleManagerActual = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await ProjectManager.DAL.Seeding.DatabaseSeeder.SeedAsync(userManager, roleManagerActual);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
