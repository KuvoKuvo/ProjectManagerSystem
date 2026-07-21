using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.Models;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.File;
using ProjectManager.BLL.Services.Project;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using System.Security.Claims;

namespace ProjectManager.API.Controllers
{
    [Authorize]
    public class ProjectsController : BaseApiController
    {
        private readonly IProjectService _projectService;
        private readonly ILocalFileService _fileService;

        public ProjectsController(IProjectService projectService, ILocalFileService fileService)
        {
            _projectService = projectService;
            _fileService = fileService;
        }

        // GET: api/projects?pageNumber=1&pageSize=10&sortBy=Name
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectDto>>> GetProjects([FromQuery] ProjectQueryParameters parameters)
        {
            var result = await _projectService.GetProjectsAsync(parameters, CurrUserId, CurrUserRole);
            return Ok(result);
        }

        // GET: api/projects/{id}
        // Requirements check: Returns complete details including assigned employees list
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDetailsDto>> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
                return NotFound(new { Message = $"Project with ID {id} not found." });

            return Ok(project);
        }

        // POST: api/projects
        // Requirements check: Collects all wizard steps data, including EmployeeIds array
        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Director)]
        public async Task<ActionResult<ProjectDetailsDto>> Create([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdProject = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, createdProject);
        }

        // PUT: api/projects/{id}
        // Requirements check: Updates general data and synchronizes delta for assigned employees
        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest(new { Message = "Route ID does not match Body ID." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound(new { Message = $"Project with ID {id} not found." });

            if (CurrUserRole != ApplicationRoles.Director && project.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "Only the assigned Project Manager or Director can edit this project." });
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
        [Authorize(Roles = ApplicationRoles.Director)]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound(new { Message = $"Project with ID {id} not found." });

            await _projectService.DeleteAsync(id);
            return NoContent();
        }

        // POST: api/projects/{id}/employees/{employeeId}
        // Requirements check: An endpoint for instant assignment of an individual employee through user interface actions
        [HttpPost("{id:int}/employees/{employeeId:int}")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<IActionResult> AssignEmployee(int id, int employeeId)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound(new { Message = $"Project with ID {id} not found." });

            if (CurrUserRole != ApplicationRoles.Director && project.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "You are not the manager of this project." });
            }

            await _projectService.AssignEmployeeAsync(id, employeeId);
            return Ok(new { Message = "Employee successfully assigned to the project." });
        }

        // DELETE: api/projects/{id}/employees/{employeeId}
        // Requirements check: The endpoint for instant cancellation of an individual employee's appointment
        [HttpDelete("{id:int}/employees/{employeeId:int}")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<IActionResult> RemoveEmployee(int id, int employeeId)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound(new { Message = $"Project with ID {id} not found." });

            if (CurrUserRole != ApplicationRoles.Director && project.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "You are not the manager of this project." });
            }

            await _projectService.RemoveEmployeeAsync(id, employeeId);
            return NoContent();
        }

        // POST: api/projects/5/documents
        [HttpPost("{id:int}/documents")]
        [Authorize(Roles = $"{ApplicationRoles.Director},{ApplicationRoles.ProjectManager}")]
        public async Task<ActionResult<ProjectDocumentDto>> UploadDocument(int id, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { Message = "No file uploaded." });

            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound(new { Message = $"Project with ID {id} not found." });

            if (CurrUserRole != ApplicationRoles.Director && project.ProjectManagerId != CurrUserId)
            {
                return StatusCode(403, new { Message = "You are not the manager of this project." });
            }

            var relativePath = await _fileService.SaveFileAsync(file);
            var resultDto = await _projectService.AddDocumentAsync(id, file.FileName, relativePath);
            return Ok(resultDto);
        }


        [HttpGet("{id:int}/documents/{documentId:int}")]
        public async Task<IActionResult> DownloadDocument(int id, int documentId)
        {
            var fileModel = await _projectService.GetDocumentForDownloadAsync(id, documentId);

            if (fileModel == null)
                return NotFound(new { Message = "Document not found." });

            return File(fileModel.Bytes, fileModel.ContentType, fileModel.FileName);
        }

    }
}
