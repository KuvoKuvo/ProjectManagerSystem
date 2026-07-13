using FluentAssertions;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.BLL.Services.Employee;
using ProjectManager.DAL.Entities;
using ProjectManager.Tests.Helpers;
using Xunit;

namespace ProjectManager.Tests.Services
{
    public class EmployeeServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task SearchAsync_ShouldReturnMatchingEmployees_WhenTermMatchesPartially()
        {
            // Arrange
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            context.Employees.AddRange(new List<Employee>()
            {
                new Employee { Id = 1, FirstName = "Ivan", LastName = "Ivanov", Email = "ivan@test.com", MiddleName = "Ivanovich", PasswordHash = "hash1", IsTemporaryPassword = false },
                new Employee { Id = 2, FirstName = "Petr", LastName = "Petrov", Email = "petr@test.com", MiddleName = "Petrovich", PasswordHash = "hash2", IsTemporaryPassword = false },
                new Employee { Id = 3, FirstName = "Sidor", LastName = "Sidorov", Email = "sidor@ivan-company.com", MiddleName = "Sidorovich", PasswordHash = "hash3", IsTemporaryPassword = false }
            });
            await context.SaveChangesAsync();

            var service = new EmployeeService(context, mapper);

            // Act
            var result = await service.SearchAsync("ivan");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(e => e.Id).Should().Contain(new[] { 1, 3 });
            result.Select(e => e.Id).Should().NotContain(2);
        }
    }
}
