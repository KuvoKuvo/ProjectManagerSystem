using FluentAssertions;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.BLL.Services.Task;
using ProjectManager.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Tests.Services
{
    public class TaskServiceTests
    {

        [Fact]
        public async System.Threading.Tasks.Task CreateAsync_ShouldAddNewTaskToDatabase_AndReturnDto()
        {
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();
            var service = new TaskService(context, mapper);

            var createDto = new TaskCreateDto
            {
                Name = "Настроить CI/CD pipeline",
                Comment = "Нужно использовать GitHub Actions",
                Priority = 4,
                Status = ProjectManager.DAL.Entities.TaskStatus.ToDo,
                ProjectId = 1,
                AuthorId = 2,
                AssigneeId = 3
            };

            var resultDto = await service.CreateAsync(createDto);


            // 1. Check that the method does not return null
            resultDto.Should().NotBeNull();

            // 2. Check that the ID has been generated (greater than 0)
            resultDto.Id.Should().BeGreaterThan(0);

            // 3. Verify that the fields are mapped correctly in the returned DTO
            resultDto.Name.Should().Be("Настроить CI/CD pipeline");
            resultDto.Priority.Should().Be(4);

            // 4. Verify that the task was actually saved to the database
            var taskInDb = await context.Tasks.FindAsync(resultDto.Id);
            taskInDb.Should().NotBeNull();
            taskInDb!.Name.Should().Be(createDto.Name);
            taskInDb.Comment.Should().Be(createDto.Comment);
            taskInDb.ProjectId.Should().Be(createDto.ProjectId);
        }
    }
}
