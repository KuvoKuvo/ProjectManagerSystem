using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Repositories.ProjectTasks
{
    public class TaskRepository : Repository<ProjectTask>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context) { }

        public (IQueryable<ProjectTask> Query, Task<int> TotalCountTask) GetTasksQuery(
            TaskQueryParameters parameters,
            int? currUserId = null,
            string? userRole = null)
        {
            var query = Context.Tasks.AsQueryable();

            if (!string.IsNullOrEmpty(userRole) && currUserId.HasValue)
            {
                if (userRole == ApplicationRoles.ProjectManager)
                {
                    query = query.Where(t => t.Project.ProjectManagerId == currUserId.Value || t.AssigneeId == currUserId.Value);
                }
                else if (userRole == ApplicationRoles.Employee)
                {
                    query = query.Where(t => t.AssigneeId == currUserId.Value);
                }
            }

            if (parameters.Status.HasValue)
                query = query.Where(t => t.Status == parameters.Status.Value);

            if (parameters.ProjectId.HasValue)
                query = query.Where(t => t.ProjectId == parameters.ProjectId.Value);

            query = parameters.SortBy?.ToLower() switch
            {
                "name" => parameters.IsDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "status" => parameters.IsDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                _ => parameters.IsDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority)
            };

            var totalCount = query.CountAsync();

            return (query, totalCount);
        }
        public async Task<ProjectTask?> GetTaskWithDetailsByIdAsync(int id)
        {
            return await Context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
