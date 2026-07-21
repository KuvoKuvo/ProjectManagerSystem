using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.UnitOfWork;
using System.Security.Cryptography;
using System.Text;

namespace ProjectManager.BLL.Services.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ProjectManager.DAL.Entities.Employee> _userManager;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ProjectManager.DAL.Entities.Employee> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employeesWithRoles = await _unitOfWork.Employees.GetAllWithRolesAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithRoles);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employeeWithRole = await _unitOfWork.Employees.GetByIdWithRoleAsync(id);
            return _mapper.Map<EmployeeDto>(employeeWithRole);
        }

        /// <summary>
        /// Requirements check: Used for AJAX partial text search on frontend steps 3 & 4.
        /// </summary>

        public async Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTrim)
        {
            var filteredWithRoles = await _unitOfWork.Employees.SearchWithRolesAsync(searchTrim);
            return _mapper.Map<IEnumerable<EmployeeDto>>(filteredWithRoles);
        }

        public async Task<EmployeeCreatedResponseDto> CreateAsync(EmployeeCreateDto dto)
        {
            var identityUserExists = await _userManager.FindByEmailAsync(dto.Email);
            if (identityUserExists != null)
            {
                throw new ArgumentException($"The employee with the Email '{dto.Email}' has already been registered.");
            }

            string tempPassword = GenerateTemporaryPassword();

            var employee = _mapper.Map<ProjectManager.DAL.Entities.Employee>(dto);
            employee.UserName = dto.Email;
            employee.EmailConfirmed = true;
            employee.IsTemporaryPassword = true;

            var identityResult = await _userManager.CreateAsync(employee, tempPassword);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new Exception($"Error in creating employee account: {errors}");
            }

            await _userManager.AddToRoleAsync(employee, dto.Role);

            return new EmployeeCreatedResponseDto
            {
                Employee = _mapper.Map<EmployeeDto>(employee),
                TemporaryPassword = tempPassword
            };
        }

        public async System.Threading.Tasks.Task UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _userManager.FindByIdAsync(dto.Id.ToString());
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {dto.Id} not found.");
            }

            _mapper.Map(dto, employee);
            employee.UserName = dto.Email;

            var result = await _userManager.UpdateAsync(employee);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update employee: {errors}");
            }

            var currentRoles = await _userManager.GetRolesAsync(employee);
            if (!currentRoles.Contains(dto.Role))
            {
                await _userManager.RemoveFromRolesAsync(employee, currentRoles);
                await _userManager.AddToRoleAsync(employee, dto.Role);
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var employee = await _userManager.FindByIdAsync(id.ToString());
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            }

            // Проверки бизнес-логики вынесены в специализированные методы репозитория в DAL
            var isProjectManager = await _unitOfWork.Employees.IsEmployeeProjectManagerAsync(id);
            if (isProjectManager)
            {
                throw new InvalidOperationException(
                    "Cannot delete this employee. They are currently assigned as a Project Manager on one or more projects. " +
                    "Please reassign the project manager role first."
                );
            }

            var hasActiveTasks = await _unitOfWork.Employees.HasActiveTasksAsync(id);
            if (hasActiveTasks)
            {
                throw new InvalidOperationException(
                    "Cannot delete this employee. They have active tasks (To Do or In Progress) assigned to them. " +
                    "Please reassign or close these tasks first."
                );
            }

            var projectRelations = await _unitOfWork.Employees.GetProjectEmployeesByEmployeeIdAsync(id);
            if (projectRelations.Any())
            {
                _unitOfWork.Employees.RemoveRange((IEnumerable<DAL.Entities.Employee>)projectRelations);
                await _unitOfWork.CompleteAsync();
            }

            var result = await _userManager.DeleteAsync(employee);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to delete employee: {errors}");
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string uppercase = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specials = "!@#$%^*";

            var random = new Random();

            var passwordChars = new List<char>
            {
                uppercase[random.Next(uppercase.Length)],
                lowercase[random.Next(lowercase.Length)],
                digits[random.Next(digits.Length)],
                specials[random.Next(specials.Length)]
            };

            string allChars = uppercase + lowercase + digits + specials;
            for (int i = 0; i < 6; i++)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
        }

        public async Task<IEnumerable<EmployeeDto>> GetEligibleManagersAsync()
        {
            var pms = await _userManager.GetUsersInRoleAsync("ProjectManager");
            var directors = await _userManager.GetUsersInRoleAsync("Director");

            var allManagers = pms.Concat(directors)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            return _mapper.Map<IEnumerable<EmployeeDto>>(allManagers);
        }
    }
}
