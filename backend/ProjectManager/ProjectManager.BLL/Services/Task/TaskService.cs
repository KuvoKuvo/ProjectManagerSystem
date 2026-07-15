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

        public async Task<IEnumerable<TaskDto>> GetTasksAsync(TaskQueryParameters parameters, int? currUserId = null, string? userRole = null)
        {
            var query = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .AsQueryable();

            //Role-based security filters
            if (!string.IsNullOrEmpty(userRole) && currUserId.HasValue)
            {
                if (userRole == "ProjectManager")
                {
                    query = query.Where(t =>
                        t.Project.ProjectManagerId == currUserId.Value ||
                        t.AssigneeId == currUserId.Value
                    );
                }
                else if (userRole == "Employee")
                {
                    query = query.Where(t => t.AssigneeId == currUserId.Value);
                }
            }

            //Apply Filters
            if (parameters.Status.HasValue)
                query = query.Where(t => t.Status == parameters.Status.Value);

            if (parameters.ProjectId.HasValue)
                query = query.Where(t => t.ProjectId == parameters.ProjectId.Value);

            //Apply Sorting
            query = parameters.SortBy.ToLower() switch
            {
                "name" => parameters.IsDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "status" => parameters.IsDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                _ => parameters.IsDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority)
            };

            var tasks = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id);

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateAsync(TaskCreateDto dto)
        {
            var task = _mapper.Map<DAL.Entities.Task>(dto);

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(task.Id) ?? _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if(task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task UpdateAsync(TaskUpdateDto dto)
        {
            var task = await _context.Tasks.FindAsync(dto.Id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {dto.Id} not found.");

            _mapper.Map(dto, task);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Requirements check: Allows quick status workflow changes (ToDo -> InProgress -> Done).
        /// </summary>
        public async System.Threading.Tasks.Task UpdateStatusAsync(int taskId, DAL.Entities.TaskStatus status)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            task.Status = status;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Requirements check: Allows fluid task re-assignment among project team members.
        /// </summary>
        public async System.Threading.Tasks.Task ChangeAssigneeAsync(int taskId, int assigneeId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            task.AssigneeId = assigneeId;
            await _context.SaveChangesAsync();
        }
    }
}
