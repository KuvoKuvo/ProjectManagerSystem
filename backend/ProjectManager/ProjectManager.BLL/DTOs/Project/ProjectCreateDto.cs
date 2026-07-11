using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Project
{
    /// <summary>
    /// Data Transfer Object for creating or updating an project record.
    /// </summary>
    public class ProjectCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string CustomerCompany { get; set; } = string.Empty;
        public string ExecutorCompany { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Priority { get; set; }
        public int ProjectManagerId { get; set; }
        public List<int> EmployeeIds { get; set; } = new();
    }
}
