using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Models
{
    /// <summary>
    /// Encapsulates parameters for filtering and sorting tasks.
    /// </summary>
    public class TaskQueryParameters : QueryParameters
    {
        public ProjectManager.DAL.Entities.TaskStatus? Status { get; set; }
        public int? ProjectId { get; set; }
        public string SortBy { get; set; } = "Priority";
        public bool IsDescending { get; set; } = false;
    }
}
