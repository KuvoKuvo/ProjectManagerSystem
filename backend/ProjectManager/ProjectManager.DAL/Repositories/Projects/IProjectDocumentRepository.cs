using ProjectManager.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Repositories.Projects
{
    public interface IProjectDocumentRepository : IRepository<ProjectDocument>
    {
        Task<ProjectDocument?> GetDocumentByIdAsync(int id);
    }
}
