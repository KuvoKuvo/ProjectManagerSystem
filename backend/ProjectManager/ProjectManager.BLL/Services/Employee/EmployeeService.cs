using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using System.Security.Cryptography;
using System.Text;

namespace ProjectManager.BLL.Services.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<DAL.Entities.Employee> _userManager;

        public EmployeeService(AppDbContext context, IMapper mapper, UserManager<DAL.Entities.Employee> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _userManager.Users.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _userManager.FindByIdAsync(id.ToString());
            return _mapper.Map<EmployeeDto>(employee);
        }

        /// <summary>
        /// Requirements check: Used for AJAX partial text search on frontend steps 3 & 4.
        /// </summary>

        public async Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTrim)
        {
            if(string.IsNullOrEmpty(searchTrim))
                return Enumerable.Empty<EmployeeDto>();

            var pattern = $"%{searchTrim.Trim()}%";

            var filteredEmployees = await _userManager.Users
                .Where(e => Microsoft.EntityFrameworkCore.EF.Functions.Like(e.FirstName, pattern) ||
                            Microsoft.EntityFrameworkCore.EF.Functions.Like(e.LastName, pattern) ||
                            Microsoft.EntityFrameworkCore.EF.Functions.Like(e.MiddleName, pattern) ||
                            Microsoft.EntityFrameworkCore.EF.Functions.Like(e.Email, pattern) ||
                            Microsoft.EntityFrameworkCore.EF.Functions.Like(e.FirstName + " " + e.LastName, pattern) ||
                            Microsoft.EntityFrameworkCore.EF.Functions.Like(e.LastName + " " + e.FirstName, pattern))
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(filteredEmployees);
        }

        public async Task<EmployeeCreatedResponseDto> CreateAsync(EmployeeCreateDto dto)
        {
            var identityUserExists = await _userManager.FindByEmailAsync(dto.Email);
            if (identityUserExists != null)
            {
                throw new ArgumentException($"The employee with the Email '{dto.Email}' has already been registered.");
            }

            string tempPassword = GenerateTemporaryPassword();

            var employee = _mapper.Map<DAL.Entities.Employee>(dto);
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
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var employee = await _userManager.FindByIdAsync(id.ToString());
            if (employee != null)
            {
                await _userManager.DeleteAsync(employee);
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^*";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
