using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.BLL.Mapping;
using ProjectManager.BLL.Services.Employee;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace ProjectManager.Tests.Services
{
    public class EmployeeServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<UserManager<Employee>> _userManagerMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            var storeMock = new Mock<IUserStore<Employee>>();
            _userManagerMock = new Mock<UserManager<Employee>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _employeeService = new EmployeeService(_context, _mapper, _userManagerMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnEmployeeDto_WhenEmployeeExists()
        {
            var targetEmployee = new Employee
            {
                Id = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                Email = "ivan@example.com",
                UserName = "ivan@example.com"
            };

            await _context.Users.AddAsync(targetEmployee);
            await _context.SaveChangesAsync();

            var result = await _employeeService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(targetEmployee.Id, result.Id);
            Assert.Equal("Иван", result.FirstName);
            Assert.Equal("Иванов", result.LastName);
            Assert.Equal("Иванович", result.MiddleName);
            Assert.Equal("ivan@example.com", result.Email);
            Assert.Equal("Employee", result.Role);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateAsync_ShouldThrowArgumentException_WhenEmailAlreadyExists()
        {
            var existingEmail = "existing@example.com";
            var existingEmployee = new Employee { Id = 5, Email = existingEmail, UserName = existingEmail };

            _userManagerMock.Setup(um => um.FindByEmailAsync(existingEmail))
                .ReturnsAsync(existingEmployee);

            var dtoToCreate = new EmployeeCreateDto
            {
                FirstName = "Петр",
                LastName = "Петров",
                Email = existingEmail,
                Role = "Employee"
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _employeeService.CreateAsync(dtoToCreate)
            );

            Assert.Contains($"The employee with the Email '{existingEmail}' has already been registered.", exception.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}