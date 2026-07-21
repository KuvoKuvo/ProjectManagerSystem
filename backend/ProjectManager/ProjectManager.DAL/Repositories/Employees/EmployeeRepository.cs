using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Repositories.Employees
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<EmployeeWithRole>> GetAllWithRolesAsync()
        {
            return await (
                from user in Context.Users
                join userRole in Context.UserRoles on user.Id equals userRole.UserId into ur
                from subUserRole in ur.DefaultIfEmpty()
                join role in Context.Roles on subUserRole.RoleId equals role.Id into r
                from subRole in r.DefaultIfEmpty()
                select new EmployeeWithRole
                {
                    Employee = user,
                    RoleName = subRole.Name ?? "Employee"
                }
            ).ToListAsync();
        }

        public async Task<EmployeeWithRole?> GetByIdWithRoleAsync(int id)
        {
            return await (
                from user in Context.Users
                where user.Id == id
                join userRole in Context.UserRoles on user.Id equals userRole.UserId into ur
                from subUserRole in ur.DefaultIfEmpty()
                join role in Context.Roles on subUserRole.RoleId equals role.Id into r
                from subRole in r.DefaultIfEmpty()
                select new EmployeeWithRole
                {
                    Employee = user,
                    RoleName = subRole.Name ?? "Employee"
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EmployeeWithRole>> SearchWithRolesAsync(string searchTrim)
        {
            if (string.IsNullOrEmpty(searchTrim))
                return Enumerable.Empty<EmployeeWithRole>();

            var pattern = $"%{searchTrim.Trim()}%";

            return await (
                from user in Context.Users
                where EF.Functions.Like(user.FirstName, pattern) ||
                      EF.Functions.Like(user.LastName, pattern) ||
                      EF.Functions.Like(user.MiddleName, pattern) ||
                      EF.Functions.Like(user.Email, pattern) ||
                      EF.Functions.Like(user.FirstName + " " + user.LastName, pattern) ||
                      EF.Functions.Like(user.LastName + " " + user.FirstName, pattern)
                join userRole in Context.UserRoles on user.Id equals userRole.UserId into ur
                from subUserRole in ur.DefaultIfEmpty()
                join role in Context.Roles on subUserRole.RoleId equals role.Id into r
                from subRole in r.DefaultIfEmpty()
                select new EmployeeWithRole
                {
                    Employee = user,
                    RoleName = subRole.Name ?? "Employee"
                }
            ).ToListAsync();
        }

        public async Task<bool> IsEmployeeProjectManagerAsync(int employeeId)
        {
            return await Context.Projects.AnyAsync(p => p.ProjectManagerId == employeeId);
        }

        public async Task<bool> HasActiveTasksAsync(int employeeId)
        {
            return await Context.Tasks.AnyAsync(t =>
                t.AssigneeId == employeeId &&
                (t.Status == Entities.TaskStatus.ToDo || t.Status == Entities.TaskStatus.InProgress)
            );
        }

        public async Task<IEnumerable<ProjectEmployee>> GetProjectEmployeesByEmployeeIdAsync(int employeeId)
        {
            return await Context.ProjectEmployees
                .Where(pe => pe.EmployeeId == employeeId)
                .ToListAsync();
        }
    }
}
