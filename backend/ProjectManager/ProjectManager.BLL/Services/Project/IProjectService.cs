using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.Project
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsAsync(ProjectQueryParameters parameters, int? currUserId = null,
            string? userRole = null);
        Task<ProjectDetailsDto?> GetByIdAsync(int id);
        Task<ProjectDetailsDto> CreateAsync(ProjectCreateDto dto);
        Task<ProjectDocumentDto> AddDocumentAsync(int projectId, string fileName, string filePath);
        System.Threading.Tasks.Task UpdateAsync(ProjectUpdateDto dto);
        System.Threading.Tasks.Task DeleteAsync(int id);

        System.Threading.Tasks.Task AssignEmployeeAsync(int projectId, int employeeId);
        System.Threading.Tasks.Task RemoveEmployeeAsync(int projectId, int employeeId);
    }
}
