using Microsoft.AspNetCore.Identity;
using ProjectManager.DAL.Entities;
using System;

namespace ProjectManager.DAL.Seeding
{
    /// <summary>
    /// Provides functionality to seed the database with initial roles and a default administrator.
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async System.Threading.Tasks.Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            string[] roleNames = { "Director", "ProjectManager", "Employee" };

            foreach(var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            const string adminEmail = "admin@projectmanager.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var defaultAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail, 
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(defaultAdmin, "SecurePass123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "Director");
                }
            }
        }
    }
}
