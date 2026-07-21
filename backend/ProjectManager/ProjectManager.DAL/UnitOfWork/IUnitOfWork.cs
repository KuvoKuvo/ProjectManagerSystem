using ProjectManager.DAL.Repositories.Employees;
using ProjectManager.DAL.Repositories.Projects;
using ProjectManager.DAL.Repositories.ProjectTasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }
        IEmployeeRepository Employees { get; }
        IProjectDocumentRepository ProjectDocuments { get; }

        Task<int> CompleteAsync();
    }
}
