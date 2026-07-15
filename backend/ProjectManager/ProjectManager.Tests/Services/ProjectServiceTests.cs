using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.Services.Project;
using ProjectManager.DAL.Entities;
using ProjectManager.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProjectManager.Tests.Services
{
    public class ProjectServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task CreateAsync_ShouldCreateProject_AndAssignEmployeesCorrectly()
        {
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            var manager = new Employee { Id = 10, FirstName = "John", LastName = "Doe", Email = "pm@company.com" };
            var emp1 = new Employee { Id = 21, FirstName = "Alice", LastName = "Smith", Email = "alice@company.com" };
            var emp2 = new Employee { Id = 22, FirstName = "Bob", LastName = "Johnson", Email = "bob@company.com" };

            context.Users.AddRange(manager, emp1, emp2);
            await context.SaveChangesAsync();

            var service = new ProjectService(context, mapper);

            var createDto = new ProjectCreateDto
            {
                Name = "E-Commerce Platform",
                CustomerCompany = "Retail Giant Corp",
                ExecutorCompany = "Super IT Solutions",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.AddMonths(6).Date,
                Priority = 3,
                ProjectManagerId = manager.Id,
                EmployeeIds = new List<int> { emp1.Id, emp2.Id }
            };

            var resultDetailsDto = await service.CreateAsync(createDto);

            // 1. Check the returned result
            resultDetailsDto.Should().NotBeNull();
            resultDetailsDto.Id.Should().BeGreaterThan(0);
            resultDetailsDto.Name.Should().Be("E-Commerce Platform");
            resultDetailsDto.ProjectManagerId.Should().Be(manager.Id);

            // 2. Verify that the project has been physically written to the database.
            var projectInDb = await context.Projects
                .Include(p => p.ProjectEmployees)
                .FirstOrDefaultAsync(p => p.Id == resultDetailsDto.Id);

            projectInDb.Should().NotBeNull();
            projectInDb!.CustomerCompany.Should().Be("Retail Giant Corp");
            projectInDb.ExecutorCompany.Should().Be("Super IT Solutions");

            // 3. Verify the correct creation of many-to-many relationships in the database
            projectInDb.ProjectEmployees.Should().HaveCount(2);
            projectInDb.ProjectEmployees.Select(pe => pe.EmployeeId)
                .Should().Contain(new[] { emp1.Id, emp2.Id });
        }
    }
}
