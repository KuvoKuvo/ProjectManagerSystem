using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Task
{
    /// <summary>
    /// Data Transfer Object for displaying task information.
    /// </summary>
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DAL.Entities.TaskStatus Status { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorFullName { get; set; } = string.Empty;
        public int AssigneeId { get; set; }
        public string AssigneeFullName { get; set; } = string.Empty;
    }
}
