using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Entities
{
    public class ProjectDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        // Foreign key to Project
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}

