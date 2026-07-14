using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.BLL.Models;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.Project;
using ProjectManager.BLL.Services.Task;
using ProjectManager.DAL.Entities;
using System.Security.Claims;

namespace ProjectManager.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly UserManager<Employee> _userManager;

        public TasksController(ITaskService taskService, IProjectService projectService, UserManager<Employee> userManager)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userManager = userManager;
        }

        // GET: api/tasks?projectId={id}&status=1&sortBy=priority&isDescending=false
        // Requirements check: Advanced filtering by project/status and sorting by priority
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] TaskQueryParameters parameters)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            int currentEmployeeId = user.Id;

            string userRole = "Employee";
            if (User.IsInRole("Director")) userRole = "Director";
            else if (User.IsInRole("ProjectManager")) userRole = "ProjectManager";

            var tasks = await _taskService.GetTasksAsync(parameters, currentEmployeeId, userRole);
            return Ok(tasks);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if(task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<ActionResult<TaskDto>> Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var project = await _projectService.GetByIdAsync(dto.ProjectId);
            if (project == null)
            {
                return BadRequest(new { Message = $"Target Project with ID {dto.ProjectId} does not exist." });
            }

            var user = await _userManager.GetUserAsync(User);
            var isDirector = User.IsInRole("Director");
            var isAssignedPM = project.ProjectManagerId == user?.Id;

            if (!isDirector && !isAssignedPM)
            {
                return StatusCode(403, new { Message = "Only the assigned Project Manager of this project or a Director can create tasks." });
            }

            var createdTask = await _taskService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/tasks/{id}
        // Full update for internal fields (Name, Description, Deadlines, etc.)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest(new { Message = "Route ID does not match Body ID." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            var user = await _userManager.GetUserAsync(User);
            var isDirector = User.IsInRole("Director");
            var isAssignedPM = project?.ProjectManagerId == user?.Id;

            if (!isDirector && !isAssignedPM)
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
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            var user = await _userManager.GetUserAsync(User);
            var isDirector = User.IsInRole("Director");
            var isAssignedPM = project?.ProjectManagerId == user?.Id;

            if (!isDirector && !isAssignedPM)
            {
                return StatusCode(403, new { Message = "You cannot delete tasks in this project." });
            }

            await _taskService.DeleteAsync(id);
            return NoContent();
        }

        // PATCH: api/tasks/{id}/status
        // Requirements check: Lightweight endpoint to update task state (ToDo -> InProgress -> Done)
        [HttpPatch("{id:int}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ProjectManager.DAL.Entities.TaskStatus status)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }

            var user = await _userManager.GetUserAsync(User);
            var isDirector = User.IsInRole("Director");

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            var isAssignedPM = project?.ProjectManagerId == user?.Id;

            var isAssignee = task.AssigneeId == user?.Id;

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
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<IActionResult> ChangeAssignee(int id, [FromBody] int assigneeId)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }

            var project = await _projectService.GetByIdAsync(task.ProjectId);
            var user = await _userManager.GetUserAsync(User);
            var isDirector = User.IsInRole("Director");
            var isAssignedPM = project?.ProjectManagerId == user?.Id;

            if (!isDirector && !isAssignedPM)
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
