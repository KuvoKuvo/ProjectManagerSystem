using ProjectManager.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Repositories.Employees
{
    public class EmployeeWithRole
    {
        public Employee Employee { get; set; } = null!;
        public string RoleName { get; set; } = string.Empty;
    }

    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<EmployeeWithRole>> GetAllWithRolesAsync();
        Task<EmployeeWithRole?> GetByIdWithRoleAsync(int id);
        Task<IEnumerable<EmployeeWithRole>> SearchWithRolesAsync(string searchTrim);
        Task<bool> IsEmployeeProjectManagerAsync(int employeeId);
        Task<bool> HasActiveTasksAsync(int employeeId);
        Task<IEnumerable<ProjectEmployee>> GetProjectEmployeesByEmployeeIdAsync(int employeeId);
    }
}
