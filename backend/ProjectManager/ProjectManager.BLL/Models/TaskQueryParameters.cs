using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Models
{
    /// <summary>
    /// Encapsulates parameters for filtering and sorting tasks.
    /// </summary>
    public class TaskQueryParameters
    {
        // Filtering
        public TaskStatus? Status { get; set; }
        public int? ProjectId { get; set; }

        // Sorting
        public string SortBy { get; set; } = "Priority";
        public bool IsDescending { get; set; } = false;
    }
}
