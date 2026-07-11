using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectManager.DAL.Entities
{
    /// <summary>
    /// Represents a specific task assigned within a project.
    /// </summary>
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public int Priority { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        // Foreign Key to Project
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        // Foreign Key to Author (Employee)
        public int AuthorId { get; set; }
        public virtual Employee Author { get; set; } = null!;

        // Foreign Key to Assignee (Employee)
        public int AssigneeId { get; set; }
        public virtual Employee Assignee { get; set; } = null!;
    }
}
