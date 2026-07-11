using System;
using System.Collections.Generic;
using System.Text;
using ProjectManager.DAL.Entities;

namespace ProjectManager.BLL.DTOs.Task
{
    /// <summary>
    /// Data Transfer Object for creating or updating an task record.
    /// </summary>
    public class TaskCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DAL.Entities.TaskStatus Status { get; set; } = DAL.Entities.TaskStatus.ToDo;
        public int ProjectId { get; set; }
        public int AuthorId { get; set; }
        public int AssigneeId { get; set; }
    }
}
