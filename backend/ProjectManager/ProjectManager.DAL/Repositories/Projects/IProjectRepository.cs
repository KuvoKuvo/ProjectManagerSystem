using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Repositories.Projects
{
    public interface IProjectRepository : IRepository<Project>
    {
        (IQueryable<Project> Query, Task<int> TotalCountTask) GetProjectsQuery(
            ProjectQueryParameters parameters,
            int? currUserId = null,
            string? userRole = null);

        Task<Project?> GetProjectWithDetailsByIdAsync(int id);
        Task<ProjectEmployee?> GetProjectEmployeeRelationAsync(int projectId, int employeeId);
        void AddProjectEmployeeRelation(ProjectEmployee relation);
        void RemoveProjectEmployeeRelation(ProjectEmployee relation);
    }
}
