using AribTask.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AribTask.Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<EmployeeTaskViewModel>> GetAllTasksAsync();
        Task<EmployeeTaskViewModel> GetTaskByIdAsync(int id);
        Task<IEnumerable<EmployeeTaskViewModel>> GetTasksByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeTaskViewModel>> GetTasksByManagerIdAsync(int managerId);
        Task<EmployeeTaskViewModel> CreateTaskAsync(EmployeeTaskViewModel taskViewModel, int createdById);
        Task<EmployeeTaskViewModel> UpdateTaskStatusAsync(int taskId, AribTask.Domain.Models.TaskStatus newStatus);
        Task<EmployeeTaskViewModel> UpdateTaskAssignmentAsync(int taskId, int newEmployeeId);
        Task<bool> DeleteTaskAsync(int id);
        Task<bool> TaskBelongsToEmployeeAsync(int taskId, int employeeId);
    }
} 