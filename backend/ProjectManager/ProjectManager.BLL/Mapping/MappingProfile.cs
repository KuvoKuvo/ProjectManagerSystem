using AutoMapper;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.DAL.Entities;

namespace ProjectManager.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee mappings
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeCreateDto, Employee>();
            CreateMap<EmployeeUpdateDto, Employee>();

            // Project mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.ProjectManagerFullName,
                opt => opt.MapFrom(src => $"{src.ProjectManager.LastName} {src.ProjectManager.FirstName}".Trim()));

            CreateMap<ProjectCreateDto, Project>()
                .ForMember(dest => dest.ProjectEmployees, opt => opt.Ignore());

            CreateMap<Project, ProjectDetailsDto>()
                .ForMember(dest => dest.ProjectManagerFullName,
                opt => opt.MapFrom(src => $"{src.ProjectManager.LastName} {src.ProjectManager.FirstName}".Trim()))
                .ForMember(dest => dest.AssignedEmployees,
                opt => opt.MapFrom(src => src.ProjectEmployees.Select(pe => pe.Employee)));

            CreateMap<ProjectUpdateDto, Project>()
                .ForMember(dest => dest.ProjectEmployees, opt => opt.Ignore());

            // Task mappings
            CreateMap<DAL.Entities.Task, TaskDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => $"{src.Author.LastName} {src.Author.FirstName}".Trim()))
                .ForMember(dest => dest.AssigneeFullName, opt => opt.MapFrom(src => $"{src.Assignee.LastName} {src.Assignee.FirstName}".Trim()));

            CreateMap<TaskCreateDto, DAL.Entities.Task>();
            CreateMap<TaskUpdateDto, DAL.Entities.Task>();
        }
    }
}
