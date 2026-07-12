using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Project
{
    public class ProjectDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
