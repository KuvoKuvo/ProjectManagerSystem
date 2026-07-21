using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.Models;
using ProjectManager.BLL.Services.File;
using ProjectManager.DAL;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Models;
using ProjectManager.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.Project
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILocalFileService _fileService;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILocalFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        /// <summary>
        /// Requirements check: Implements dynamic filtering, sorting, and role-based data isolation.
        /// </summary>

        public async Task<PagedResult<ProjectDto>> GetProjectsAsync(
            ProjectQueryParameters parameters,
            int? currUserId = null,
            string? userRole = null)
        {

            // 1. Get the IQueryable and the count task
            var (query, totalCountTask) = _unitOfWork.Projects.GetProjectsQuery(parameters, currUserId, userRole);

            // 2. ProjectTo will automatically generate a lightweight SQL SELECT for the ProjectDto fields.
            var items = await query
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var totalCount = await totalCountTask;

            // 3. Return the grouped result
            return new PagedResult<ProjectDto>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<ProjectDetailsDto?> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetProjectWithDetailsByIdAsync(id);
            return _mapper.Map<ProjectDetailsDto>(project);
        }

        /// <summary>
        /// Requirements check: Manually syncs wizard-driven step 4 employee assignments.
        /// </summary>
        public async Task<ProjectDetailsDto> CreateAsync(ProjectCreateDto dto)
        {
            var project = _mapper.Map<ProjectManager.DAL.Entities.Project>(dto);

            if (dto.EmployeeIds != null && dto.EmployeeIds.Any())
            {
                foreach (var empId in dto.EmployeeIds)
                {
                    project.ProjectEmployees.Add(new ProjectEmployee
                    {
                        EmployeeId = empId
                    });
                }
            }

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CompleteAsync();

            return await GetByIdAsync(project.Id) ?? _mapper.Map<ProjectDetailsDto>(project);
        }

        /// <summary>
        /// Requirements check: Gracefully syncs many-to-many relationship delta updates.
        /// </summary>

        public async System.Threading.Tasks.Task UpdateAsync(ProjectUpdateDto dto)
        {
            var project = await _unitOfWork.Projects.GetProjectWithDetailsByIdAsync(dto.Id);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID {dto.Id} not found.");

            _mapper.Map(dto, project);

            // Удаляем сотрудников, которых больше нет в списке назначения
            var toRemove = project.ProjectEmployees
                .Where(pe => !dto.EmployeeIds.Contains(pe.EmployeeId)).ToList();

            foreach (var pe in toRemove)
                _unitOfWork.Projects.RemoveProjectEmployeeRelation(pe);

            // Добавляем новых сотрудников
            var currEmployeeIds = project.ProjectEmployees.Select(pe => pe.EmployeeId).ToList();
            foreach (var empId in dto.EmployeeIds.Where(id => !currEmployeeIds.Contains(id)))
            {
                _unitOfWork.Projects.AddProjectEmployeeRelation(new ProjectEmployee
                {
                    ProjectId = dto.Id,
                    EmployeeId = empId
                });
            }

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.CompleteAsync();
        }
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project != null)
            {
                _unitOfWork.Projects.Remove(project);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async System.Threading.Tasks.Task AssignEmployeeAsync(int projectId, int employeeId)
        {
            var relation = await _unitOfWork.Projects.GetProjectEmployeeRelationAsync(projectId, employeeId);

            if (relation == null)
            {
                _unitOfWork.Projects.AddProjectEmployeeRelation(new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId
                });
                await _unitOfWork.CompleteAsync();
            }
        }

        public async System.Threading.Tasks.Task RemoveEmployeeAsync(int projectId, int employeeId)
        {
            var relation = await _unitOfWork.Projects.GetProjectEmployeeRelationAsync(projectId, employeeId);

            if (relation != null)
            {
                _unitOfWork.Projects.RemoveProjectEmployeeRelation(relation);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<ProjectDocumentDto> AddDocumentAsync(int projectId, string fileName, string filePath)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
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

            await _unitOfWork.ProjectDocuments.AddAsync(document);
            await _unitOfWork.CompleteAsync();

            return new ProjectDocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                FilePath = document.FilePath
            };
        }

        public async Task<FileDownloadModel?> GetDocumentForDownloadAsync(int projectId, int documentId)
        {
            var project = await _unitOfWork.Projects.GetProjectWithDetailsByIdAsync(projectId);
            if (project == null) return null;

            var document = project.Documents.FirstOrDefault(d => d.Id == documentId);
            if (document == null) return null;

            var fileData = await _fileService.GetFileAsync(document.FilePath);
            if (fileData == null) return null;

            return new FileDownloadModel
            {
                Bytes = fileData.Value.Bytes,
                ContentType = fileData.Value.ContentType,
                FileName = document.FileName
            };
        }
    }
}
