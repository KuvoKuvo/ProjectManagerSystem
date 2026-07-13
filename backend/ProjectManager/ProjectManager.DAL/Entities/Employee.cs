using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectManager.DAL.Entities
{
    /// <summary>
    /// Represents both the security identity and the physical employee in the system.
    /// </summary>
    public class Employee : IdentityUser<int>
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        public bool IsTemporaryPassword { get; set; } = true;


        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
        public virtual ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public virtual ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
    }
}
