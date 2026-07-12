using FluentAssertions;
using ProjectManager.BLL.Services.Task;
using ProjectManager.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Tests.Services
{
    public class TaskServiceTests
    {
        /// <summary>
        /// UpdateStatusAsync Tests
        /// </summary>

        [Fact]
        public async System.Threading.Tasks.Task UpdateStatusAsync_ShouldUpdateOnlyStatus_WhenTaskExists()
        {
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            var existingTask = new ProjectManager.DAL.Entities.Task
            {
                Id = 100,
                Name = "Test task Kanban",
                Comment = "A comment that should not change",
                Priority = 2,
                Status = DAL.Entities.TaskStatus.ToDo,
                ProjectId = 1,
                AuthorId = 5,
                AssigneeId = 5
            };

            context.Tasks.Add(existingTask);
            await context.SaveChangesAsync();

            var service = new TaskService(context, mapper);

            // ACT
            await service.UpdateStatusAsync(taskId: 100, status: DAL.Entities.TaskStatus.InProgress);

            // ASSERT
            var updatedTaskInDb = await context.Tasks.FindAsync(100);
            updatedTaskInDb.Should().NotBeNull();

            updatedTaskInDb!.Status.Should().Be(DAL.Entities.TaskStatus.InProgress);

            updatedTaskInDb.Name.Should().Be("Test task Kanban");
            updatedTaskInDb.Comment.Should().Be("A comment that should not change");
            updatedTaskInDb.Priority.Should().Be(2);
            updatedTaskInDb.ProjectId.Should().Be(1);
            updatedTaskInDb.AuthorId.Should().Be(5);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateStatusAsync_ShouldThrowKeyNotFoundException_WhenTaskDoesNotExist()
        {
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            var service = new TaskService(context, mapper);

            Func<System.Threading.Tasks.Task> act = async () =>
                await service.UpdateStatusAsync(taskId: 999, status: DAL.Entities.TaskStatus.Done);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Task with ID 999 not found.");
        }

        /// <summary>
        /// ChangeAssigneeAsync Tests
        /// </summary>

        [Fact]
        public async System.Threading.Tasks.Task ChangeAssigneeAsync_ShouldUpdateAssigneeId_WhenTaskExists()
        {
            using var context = TestHelper.CreateInMemoryDbContext();
            var mapper = TestHelper.CreateRealMapper();

            int initialAssigneeId = 1;
            int newAssigneeId = 2;

            var task = new ProjectManager.DAL.Entities.Task
            {
                Id = 50,
                Name = "Task for reassignment",
                Status = DAL.Entities.TaskStatus.ToDo,
                AssigneeId = initialAssigneeId,
                ProjectId = 1
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var service = new TaskService(context, mapper);

            await service.ChangeAssigneeAsync(taskId: 50, assigneeId: newAssigneeId);

            var taskInDb = await context.Tasks.FindAsync(50);

            taskInDb.Should().NotBeNull();
            taskInDb!.AssigneeId.Should().Be(newAssigneeId);
        }
    }
}
