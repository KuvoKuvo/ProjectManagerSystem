using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.BLL.Models;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using ProjectManager.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.Task
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Requirements check: Filters tasks by status/project and enforces security visibility.
        /// </summary>

        public async Task<PagedResult<TaskDto>> GetTasksAsync(
            TaskQueryParameters parameters,
            int? currUserId = null,
            string? userRole = null)
        {
            // 1. Get the IQueryable and the count task
            var (query, totalCountTask) = _unitOfWork.Tasks.GetTasksQuery(parameters, currUserId, userRole);

            // 2. AutoMapper will automatically convert the query into an optimal SQL SELECT statement for the TaskDto fields.
            var items = await query
                .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var totalCount = await totalCountTask;

            // 3. Return the packed result
            return new PagedResult<TaskDto>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            var task = await _unitOfWork.Tasks.GetTaskWithDetailsByIdAsync(id);
            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateAsync(TaskCreateDto dto)
        {
            var task = _mapper.Map<ProjectTask>(dto);

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.CompleteAsync();

            return await GetByIdAsync(task.Id) ?? _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task != null)
            {
                _unitOfWork.Tasks.Remove(task);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async System.Threading.Tasks.Task UpdateAsync(TaskUpdateDto dto)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(dto.Id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {dto.Id} not found.");

            _mapper.Map(dto, task);
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Requirements check: Allows quick status workflow changes (ToDo -> InProgress -> Done).
        /// </summary>
        public async System.Threading.Tasks.Task UpdateStatusAsync(int taskId, ProjectManager.DAL.Entities.TaskStatus status)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            task.Status = status;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Requirements check: Allows fluid task re-assignment among project team members.
        /// </summary>
        public async System.Threading.Tasks.Task ChangeAssigneeAsync(int taskId, int assigneeId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            task.AssigneeId = assigneeId;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.CompleteAsync();
        }
    }
}
