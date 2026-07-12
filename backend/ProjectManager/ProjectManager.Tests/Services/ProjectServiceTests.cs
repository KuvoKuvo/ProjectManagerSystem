using FluentAssertions;
using ProjectManager.BLL.Models;
using ProjectManager.BLL.Services.Project;
using ProjectManager.DAL.Entities;
using ProjectManager.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Tests.Services
{
    public class ProjectServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task GetProjectsAsync_ShouldReturnOnlyManagedProjects_ForProjectManagerRole()
        {
            // Arrange
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            int managerId = 10;
            int otherManagerId = 99;

            context.Projects.AddRange(new List<Project>
            {
                new Project { Id = 1, Name = "Manager's Owned Project", ProjectManagerId = managerId, CustomerCompany = "A", ExecutorCompany = "B" },
                new Project { Id = 2, Name = "Other Manager's Project", ProjectManagerId = otherManagerId, CustomerCompany = "A", ExecutorCompany = "B" }
            });
            await context.SaveChangesAsync();

            var service = new ProjectService(context, mapper);
            var queryParams = new ProjectQueryParameters { SortBy = "name", IsDescending = false };

            // Act
            var result = await service.GetProjectsAsync(queryParams, currUserId: managerId, userRole: "ProjectManager");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
            result.First().Name.Should().Be("Manager's Owned Project");
        }
    }
}
