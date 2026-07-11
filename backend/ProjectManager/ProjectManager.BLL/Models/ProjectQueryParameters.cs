using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Models
{
    /// <summary>
    /// Encapsulates parameters for filtering and sorting projects.
    /// </summary>
    public class ProjectQueryParameters
    {
        // Filtering
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public int? Priority { get; set; }

        // Sorting
        public string SortBy { get; set; } = "Name";
        public bool IsDescending { get; set; } = false;
    }
}
