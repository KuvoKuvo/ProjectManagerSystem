using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectManager.DAL.Entities
{
    /// <summary>
    /// Represents a commercial or internal project.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string CustomerCompany { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string ExecutorCompany { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public int Priority { get; set; }

        // Foreign Key for Project Manager (One-to-Many: One Employee can manage multiple projects)
        public int ProjectManagerId { get; set; }
        public virtual Employee ProjectManager { get; set; } = null!;

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
