using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.DAL.Repositories.Projects
{
    public class ProjectDocumentRepository : Repository<ProjectDocument>, IProjectDocumentRepository
    {
        public ProjectDocumentRepository(AppDbContext context) : base(context) { }
        public async Task<ProjectDocument?> GetDocumentByIdAsync(int id)
        {
            return await Context.ProjectDocuments.FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
