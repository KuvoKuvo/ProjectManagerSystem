using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Task
{
    /// <summary>
    /// Data Transfer Object for updating task attributes and state.
    /// </summary>
    public class TaskUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Priority { get; set; }
        public TaskStatus Status { get; set; }
        public int AssigneeId { get; set; }
    }
}
