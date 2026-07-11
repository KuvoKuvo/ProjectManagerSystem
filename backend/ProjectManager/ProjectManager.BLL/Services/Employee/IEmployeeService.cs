using ProjectManager.BLL.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.Employee
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTrim);
        Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
        System.Threading.Tasks.Task UpdateAsync(EmployeeUpdateDto dto);
        System.Threading.Tasks.Task DeleteAsync(int id);
    }
}
