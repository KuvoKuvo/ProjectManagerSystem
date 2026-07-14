using ProjectManager.BLL.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Project
{
    /// <summary>
    /// Data Transfer Object for displaying project information.
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CustomerCompany { get; set; } = string.Empty;
        public string ExecutorCompany { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Priority { get; set; }
        public int ProjectManagerId { get; set; }
        public string ProjectManagerFullName { get; set; } = string.Empty;
    }
    public class ProjectDetailsDto : ProjectDto
    {
        public EmployeeDto ProjectManager { get; set; } = null!;
        public List<EmployeeDto> AssignedEmployees { get; set; } = new();
        public List<ProjectDocumentDto> Documents { get; set; } = new();
    }
}
