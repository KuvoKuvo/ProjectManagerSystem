using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.API.Services;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.Models;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.Project;
using ProjectManager.DAL.Entities;
using System.Security.Claims;

namespace ProjectManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILocalFileService _fileService;
        private readonly UserManager<Employee> _userManager;

        public ProjectsController(IProjectService projectService, ILocalFileService fileService, UserManager<Employee> userManager)
        {
            _projectService = projectService;
            _fileService = fileService;
            _userManager = userManager;
        }

        // GET: api/projects?startDateFrom=2026-01-01&priority=2&sortBy=startDate&isDescending=true
        // Requirements check: Advanced dynamic filtering and sorting for project tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects([FromQuery] ProjectQueryParameters parameters)
        {
            var user = await _userManager.GetUserAsync(User);
            int? currentEmployeeId = user?.Id;
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var projects = await _projectService.GetProjectsAsync(parameters, currentEmployeeId, userRole);
            return Ok(projects);
        }

        // GET: api/projects/{id}
        // Requirements check: Returns complete details including assigned employees list
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDetailsDto>> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound(new { Message = $"Project with ID {id} not found." });
            }
            return Ok(project);
        }

        // POST: api/projects
        // Requirements check: Collects all wizard steps data, including EmployeeIds array
        [HttpPost]
        [Authorize(Roles = "Director")]
        public async Task<ActionResult<ProjectDetailsDto>> Create([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProject = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new
            {
                id = createdProject.Id

            }, createdProject);
        }

        // PUT: api/projects/{id}
        // Requirements check: Updates general data and synchronizes delta for assigned employees
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            if(id != dto.Id)
            {
                return BadRequest(new { Message = "Route ID does not match Body ID." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _projectService.UpdateAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound(new { Message = $"Project with ID {id} not found." });
            }

            await _projectService.DeleteAsync(id);
            return NoContent();
        }

        // POST: api/projects/{id}/employees/{employeeId}
        // Requirements check: An endpoint for instant assignment of an individual employee through user interface actions
        [HttpPost("{id:int}/employees/{employeeId:int}")]
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<IActionResult> AssignEmployee(int id, int employeeId)
        {
            try
            {
                await _projectService.AssignEmployeeAsync(id, employeeId);
                return Ok(new { Message = "Employee successfully assigned to the project." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/projects/{id}/employees/{employeeId}
        // Requirements check: The endpoint for instant cancellation of an individual employee's appointment
        [HttpDelete("{id:int}/employees/{employeeId:int}")]
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<IActionResult> RemoveEmployee(int id, int employeeId)
        {
            try
            {
                await _projectService.RemoveEmployeeAsync(id, employeeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: api/projects/5/documents
        [HttpPost("{id:int}/documents")]
        [Authorize(Roles = "Director,ProjectManager")]
        public async Task<ActionResult<ProjectDocumentDto>> UploadDocument(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "No file uploaded." });
            }

            try
            {
                var relativePath = await _fileService.SaveFileAsync(file);
                var resultDto = await _projectService.AddDocumentAsync(id, file.FileName, relativePath);

                return Ok(resultDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
