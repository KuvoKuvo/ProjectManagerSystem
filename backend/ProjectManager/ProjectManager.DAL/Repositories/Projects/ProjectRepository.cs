using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using ProjectManager.DAL.Repositories.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjectManager.DAL.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context) { }

        public (IQueryable<Project> Query, Task<int> TotalCountTask) GetProjectsQuery(
            ProjectQueryParameters parameters,
            int? currUserId = null,
            string? userRole = null)
        {
            var query = Context.Projects
                .Include(p => p.ProjectManager)
                .AsQueryable();

            if(currUserId.HasValue && userRole != ApplicationRoles.Director)
            {
                query = query.Where(p =>
                    p.ProjectManagerId == currUserId.Value ||
                    p.ProjectEmployees.Any(pe => pe.EmployeeId == currUserId.Value)
                );
            }

            if (parameters.StartDateTo.HasValue)
                query = query.Where(p => p.StartDate <= parameters.StartDateTo.Value);

            if(parameters.StartDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= parameters.StartDateFrom.Value);

            if(parameters.Priority.HasValue)
                query = query.Where(p => p.Priority == parameters.Priority.Value);

            query = parameters.SortBy?.ToLower() switch
            {
                "startdate" => parameters.IsDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                "priority" => parameters.IsDescending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                _ => parameters.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
            };

            var totalCount = query.CountAsync();

            return (query, totalCount);
        }

        public async Task<Project?> GetProjectWithDetailsByIdAsync(int id)
        {
            return await Context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProjectEmployee?> GetProjectEmployeeRelationAsync(int projectId, int employeeId)
        {
            return await Context.ProjectEmployees
                .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);
        }

        public void AddProjectEmployeeRelation(ProjectEmployee relation)
        {
            Context.ProjectEmployees.Add(relation);
        }

        public void RemoveProjectEmployeeRelation(ProjectEmployee relation)
        {
            Context.ProjectEmployees.Remove(relation);
        }
    }
}