using AutoMapper;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.BLL.DTOs.Project;
using ProjectManager.BLL.DTOs.Task;
using ProjectManager.DAL.Entities;
using ProjectManager.DAL.Repositories.Employees;

namespace ProjectManager.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // EMPLOYEE MAPPINGS
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<EmployeeWithRole, EmployeeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.Employee.MiddleName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Employee.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleName));

            CreateMap<EmployeeCreateDto, Employee>();
            CreateMap<EmployeeUpdateDto, Employee>();


            // PROJECT MAPPINGS
            CreateMap<ProjectDocument, ProjectDocumentDto>();

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.ProjectManagerFullName, opt => 
                opt.MapFrom(src => $"{src.ProjectManager.LastName} {src.ProjectManager.FirstName}".Trim()));

            CreateMap<ProjectCreateDto, Project>()
                .ForMember(dest => dest.ProjectEmployees, opt => opt.Ignore());

            CreateMap<Project, ProjectDetailsDto>()
                .ForMember(dest => dest.ProjectManagerFullName,
                    opt => opt.MapFrom(src => $"{src.ProjectManager.LastName} {src.ProjectManager.FirstName}".Trim()))
                .ForMember(dest => dest.ProjectManager,
                    opt => opt.MapFrom(src => src.ProjectManager))
                .ForMember(dest => dest.AssignedEmployees,
                    opt => opt.MapFrom(src => src.ProjectEmployees.Select(pe => pe.Employee)))
                .ForMember(dest => dest.Documents,
                    opt => opt.MapFrom(src => src.Documents));

            CreateMap<ProjectUpdateDto, Project>()
                .ForMember(dest => dest.ProjectEmployees, opt => opt.Ignore());


            // TASK MAPPINGS
            CreateMap<ProjectTask, TaskDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => $"{src.Author.LastName} {src.Author.FirstName}".Trim()))
                .ForMember(dest => dest.AssigneeFullName, opt => opt.MapFrom(src => $"{src.Assignee.LastName} {src.Assignee.FirstName}".Trim()));

            CreateMap<TaskCreateDto, ProjectTask>();
            CreateMap<TaskUpdateDto, ProjectTask>();
        }
    }
}
