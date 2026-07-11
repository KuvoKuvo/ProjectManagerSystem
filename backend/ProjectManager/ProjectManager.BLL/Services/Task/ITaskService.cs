using ProjectManager.BLL.DTOs.Task;
using ProjectManager.BLL.Models;

namespace ProjectManager.BLL.Services.Task
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksAsync(TaskQueryParameters parameters, int? currUserId = null, string? userRole = null);
        Task<TaskDto?> GetByIdAsync(int id);
        Task<TaskDto> CreateAsync(TaskCreateDto dto);
        System.Threading.Tasks.Task UpdateAsync(TaskUpdateDto dto);
        System.Threading.Tasks.Task DeleteAsync(int id);

        System.Threading.Tasks.Task UpdateStatusAsync(int taskId, DAL.Entities.TaskStatus status);
        System.Threading.Tasks.Task ChangeAssigneeAsync(int taskId, int assigneeId);
    }
}
