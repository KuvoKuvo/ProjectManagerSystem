using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.BLL.Models;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.Project;
using ProjectManager.BLL.Services.Task;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using System.Security.Claims;

namespace ProjectManager.API.Controllers
{
    [Authorize]
    public class TasksController : BaseApiController
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;

        public TasksController(ITaskService taskService, IProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        // Requirements check: Advanced filtering by project/status and sorting by priority
        // GET: api/tasks?projectId=1&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks([FromQuery] TaskQueryParameters parameters)
        {
            var result = await _taskService.GetTasksAsync(parameters, CurrUserId, CurrUserRole);
            return Ok(result);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<ActionResult<TaskDto>> Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var project = await _projectService.GetByIdAsync(dto.ProjectId);
            if (project == null) return BadRequest(new { Message = $"Target Project with ID {dto.ProjectId} does not exist." });

            if (CurrUserRole != ApplicationRoles.Director && project.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "Only the assigned Project Manager of this project or a Director can create tasks." });
            }

            var createdTask = await _taskService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/tasks/{id}
        // Full update for internal fields (Name, Description, Deadlines, etc.)
        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest(new { Message = "Route ID does not match Body ID." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            if (CurrUserRole != ApplicationRoles.Director && project?.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "You cannot modify tasks in this project." });
            }

            try
            {
                await _taskService.UpdateAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            if (CurrUserRole != ApplicationRoles.Director && project?.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "You cannot delete tasks in this project." });
            }

            await _taskService.DeleteAsync(id);
            return NoContent();
        }

        // PATCH: api/tasks/{id}/status
        // Requirements check: Lightweight endpoint to update task state (ToDo -> InProgress -> Done)
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ProjectManager.DAL.Entities.TaskStatus status)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });

            var project = await _projectService.GetByIdAsync(task.ProjectId);

            bool isDirector = CurrUserRole == ApplicationRoles.Director;
            bool isAssignedPM = project?.ProjectManagerId == CurrUserId;
            bool isAssignee = task.AssigneeId == CurrUserId;

            if (!isDirector && !isAssignedPM && !isAssignee)
            {
                return StatusCode(403, new { Message = "You can only update status of tasks assigned to you." });
            }

            try
            {
                await _taskService.UpdateStatusAsync(id, status);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // PATCH: api/tasks/{id}/assignee
        // Requirements check: Lightweight endpoint to hot-swap responsible employee
        [HttpPatch("{id:int}/assignee")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<IActionResult> ChangeAssignee(int id, [FromBody] int assigneeId)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            if (CurrUserRole != ApplicationRoles.Director && project?.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "You cannot reassign tasks in this project." });
            }

            try
            {
                await _taskService.ChangeAssigneeAsync(id, assigneeId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
