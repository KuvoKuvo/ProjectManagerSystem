using ProjectManager.DAL.Repositories;
using ProjectManager.DAL.Repositories.Employees;
using ProjectManager.DAL.Repositories.Projects;
using ProjectManager.DAL.Repositories.ProjectTasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IProjectRepository Projects { get; }
        public ITaskRepository Tasks { get; }
        public IEmployeeRepository Employees { get; }
        public IProjectDocumentRepository ProjectDocuments { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // Инициализируем репозитории
            Projects = new ProjectRepository(_context);
            Tasks = new TaskRepository(_context);
            Employees = new EmployeeRepository(_context);
            ProjectDocuments = new ProjectDocumentRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
