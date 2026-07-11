using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Entities
{
    /// <summary>
    /// Join entity for the Many-to-Many relationship between Projects and Employees.
    /// </summary>
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;
    }
}
