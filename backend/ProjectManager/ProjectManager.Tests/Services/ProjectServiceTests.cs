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
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            int managerId = 10;
            int otherManagerId = 99;

            var manager1 = new Employee { Id = managerId, FirstName = "Ivan", LastName = "Manager", Email = "m1@test.com", MiddleName = "Ivanovich" };
            var manager2 = new Employee { Id = otherManagerId, FirstName = "Petr", LastName = "OtherManager", Email = "m2@test.com", MiddleName = "Petrovich" };

            context.Employees.AddRange(manager1, manager2);
            await context.SaveChangesAsync();

            context.Projects.AddRange(new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Name = "Manager's Owned Project",
                    ProjectManagerId = managerId,
                    CustomerCompany = "Company A",
                    ExecutorCompany = "Company B",
                    StartDate = DateTime.UtcNow,
                    Priority = 1,
                    ProjectEmployees = new List<ProjectEmployee>()
                },
                new Project
                {
                    Id = 2,
                    Name = "Other Manager's Project",
                    ProjectManagerId = otherManagerId,
                    CustomerCompany = "Company C",
                    ExecutorCompany = "Company D",
                    StartDate = DateTime.UtcNow,
                    Priority = 2,
                    ProjectEmployees = new List<ProjectEmployee>()
                }
            });
            await context.SaveChangesAsync();

            var service = new ProjectService(context, mapper);

            var queryParams = new ProjectQueryParameters
            {
                SortBy = "startdate",
                IsDescending = false
            };

            var result = await service.GetProjectsAsync(queryParams, currUserId: managerId, userRole: "ProjectManager");

            result.Should().NotBeNull();
            result.Should().HaveCount(1, "because ProjectManager role must filter out projects belonging to other managers");

            var singleProject = result.First();
            singleProject.Id.Should().Be(1);
            singleProject.Name.Should().Be("Manager's Owned Project");
        }
    }
}
