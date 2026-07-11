using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ProjectManager.DAL.Entities
{
    /// <summary>
    /// Custom identity user linked to the physical Employee entity.
    /// </summary>
    public class ApplicationUser : IdentityUser<int>
    {
        // One-to-One relationship: Every login account belongs to one employee record
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
