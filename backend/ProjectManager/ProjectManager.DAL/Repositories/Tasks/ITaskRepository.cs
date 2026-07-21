using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using TaskStatus = ProjectManager.DAL.Entities.TaskStatus;

namespace ProjectManager.DAL.Repositories.ProjectTasks
{
    public interface ITaskRepository : IRepository<ProjectTask>
    {
        (IQueryable<ProjectTask> Query, Task<int> TotalCountTask) GetTasksQuery(
            TaskQueryParameters parameters,
            int? currUserId = null,
            string? userRole = null);
        Task<ProjectTask?> GetTaskWithDetailsByIdAsync(int id);
    }
}
