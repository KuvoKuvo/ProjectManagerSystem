using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.BLL.Models;
using ProjectManager.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.Task
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Requirements check: Filters tasks by status/project and enforces security visibility.
        /// </summary>

        public Task<IEnumerable<TaskDto>> GetTasksAsync(TaskQueryParameters parameters, int? currUserId = null, string? userRole = null)
        {
            var query = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userRole) && currentUserId.HasValue)
            {
                if (userRole == "ProjectManager")
                {
                    query = query.Where(t => t.Project.ProjectManagerId == currentUserId);
                }
                else if (userRole == "Employee")
                {

                }
            }
        }
        public System.Threading.Tasks.Task ChangeAssigneeAsync(int taskId, int assigneeId)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDto> CreateAsync(TaskCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateAsync(TaskUpdateDto dto)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateStatusAsync(int taskId, DAL.Entities.TaskStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
