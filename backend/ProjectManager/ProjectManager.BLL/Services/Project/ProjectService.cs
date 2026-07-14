using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.Models;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.Project
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Requirements check: Implements dynamic filtering, sorting, and role-based data isolation.
        /// </summary>

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(ProjectQueryParameters parameters, int? currUserId = null, string? userRole = null)
        {
            var query = _context.Projects.Include(p => p.ProjectManager).AsQueryable();

            // Role-based security filter (Senior-style rewrite)
            if (currUserId.HasValue)
            {
                var normalizedRole = userRole?.Trim().ToLower();

                if (normalizedRole != "director")
                {
                    query = query.Where(p =>
                        p.ProjectManagerId == currUserId.Value ||
                        p.ProjectEmployees.Any(pe => pe.EmployeeId == currUserId.Value)
                    );
                }
            }

            // Dynamic Filtering
            if (parameters.StartDateTo.HasValue)
                query = query.Where(p => p.StartDate <= parameters.StartDateTo.Value);

            if (parameters.StartDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= parameters.StartDateFrom.Value);

            if (parameters.Priority.HasValue)
                query = query.Where(p => p.Priority == parameters.Priority.Value);

            // Dynamic Sorting
            query = parameters.SortBy.ToLower() switch
            {
                "startdate" => parameters.IsDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                "priority" => parameters.IsDescending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                _ => parameters.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
            };

            var projects = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDetailsDto?> GetByIdAsync(int id)
        {
            var projects = await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<ProjectDetailsDto>(projects);
        }

        /// <summary>
        /// Requirements check: Manually syncs wizard-driven step 4 employee assignments.
        /// </summary>
        public async Task<ProjectDetailsDto> CreateAsync(ProjectCreateDto dto)
        {
            var projects = _mapper.Map<DAL.Entities.Project>(dto);

            if(dto.EmployeeIds != null && dto.EmployeeIds.Any())
            {
                foreach(var empId in dto.EmployeeIds)
                {
                    projects.ProjectEmployees.Add(new ProjectEmployee
                    {
                        EmployeeId = empId
                    });
                }
            }

            _context.Projects.Add(projects);
            await _context.SaveChangesAsync();

            // Return full details including nested entities
            return await GetByIdAsync(projects.Id) ?? _mapper.Map<ProjectDetailsDto>(projects);
        }

        /// <summary>
        /// Requirements check: Gracefully syncs many-to-many relationship delta updates.
        /// </summary>

        public async System.Threading.Tasks.Task UpdateAsync(ProjectUpdateDto dto)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectEmployees)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if(project == null)
                throw new KeyNotFoundException($"Project with ID {dto.Id} not found.");

            _mapper.Map(dto, project);

            //Remove employees that are no longer assigned
            var toRemove = project.ProjectEmployees
                .Where(pe => !dto.EmployeeIds.Contains(pe.EmployeeId)).ToList();

            foreach (var pe in toRemove)
                project.ProjectEmployees.Remove(pe);

            //Add newly assigned employees
            var currEmployeeIds = project.ProjectEmployees.Select(pe => pe.EmployeeId).ToList();
            foreach(var empId in dto.EmployeeIds.Where(id => !currEmployeeIds.Contains(id)))
            {
                project.ProjectEmployees.Add(new ProjectEmployee
                {
                    ProjectId = dto.Id,
                    EmployeeId = empId
                });
            }
            await _context.SaveChangesAsync();
                
        }
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if(project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task AssignEmployeeAsync(int projectId, int employeeId)
        {
            var exists = await _context.ProjectEmployees
                .AnyAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

            if (!exists)
            {
                _context.ProjectEmployees.Add(new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId
                });
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task RemoveEmployeeAsync(int projectId, int employeeId)
        {
            var record = await _context.ProjectEmployees
                .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

            if (record != null)
            {
                _context.ProjectEmployees.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProjectDocumentDto> AddDocumentAsync(int projectId, string fileName, string filePath)
        {
            // Check if project exists
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {projectId} not found.");
            }

            var document = new ProjectDocument
            {
                ProjectId = projectId,
                FileName = fileName,
                FilePath = filePath
            };

            _context.ProjectDocuments.Add(document);
            await _context.SaveChangesAsync();

            // Map to DTO manually or via AutoMapper
            return new ProjectDocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                FilePath = document.FilePath
            };
        }

        public async Task<ProjectDocumentDto?> GetDocumentByIdAsync(int documentId)
        {
            var document = await _context.ProjectDocuments
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null) return null;

            return new ProjectDocumentDto
            { 
                Id = document.Id,
                FileName = document.FileName,
                FilePath = document.FilePath
            };
        }

    }
}
