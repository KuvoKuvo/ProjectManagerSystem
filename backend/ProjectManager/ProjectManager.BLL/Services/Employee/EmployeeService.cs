using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;

namespace ProjectManager.BLL.Services.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _context.Employees.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            return _mapper.Map<EmployeeDto>(employee);
        }

        /// <summary>
        /// Requirements check: Used for AJAX partial text search on frontend steps 3 & 4.
        /// </summary>

        public async Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTrim)
        {
            if(string.IsNullOrEmpty(searchTrim))
                return Enumerable.Empty<EmployeeDto>();

            var lowerTrim = searchTrim.ToLower();

            var filtredEmployees = await _context.Employees
                .Where(e => e.FirstName.ToLower().Contains(lowerTrim) ||
                e.LastName.ToLower().Contains(lowerTrim) ||
                e.MiddleName.ToLower().Contains(lowerTrim) ||
                e.Email.ToLower().Contains(lowerTrim))
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(filtredEmployees);
        }

        public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
        {
            var employee = _mapper.Map<DAL.Entities.Employee>(dto);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return  _mapper.Map<EmployeeDto>(employee);
        }

        public async System.Threading.Tasks.Task UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(dto.Id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {dto.Id} not found.");
            }

            _mapper.Map(dto, employee);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if(employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }       
    }
}
