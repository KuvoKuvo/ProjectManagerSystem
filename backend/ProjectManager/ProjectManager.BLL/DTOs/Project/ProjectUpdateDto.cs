using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Project
{
    /// <summary>
    /// Data Transfer Object for updating project baseline information.
    /// </summary>
    public class ProjectUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CustomerCompany { get; set; } = string.Empty;
        public string ExecutorCompany { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Priority { get; set; }
        public int ProjectManagerId { get; set; }

        // List of employee IDs assigned to this project after update
        public List<int> EmployeeIds { get; set; } = new();
    }
}
