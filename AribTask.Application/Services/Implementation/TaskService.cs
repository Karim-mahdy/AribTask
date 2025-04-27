using AribTask.Application.Common.Abstraction;
using AribTask.Application.Services.Interfaces;
using AribTask.Application.ViewModels;
using AribTask.Domain.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AribTask.Application.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeTaskViewModel>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.EmployeeTasks
                .GetAllAsync(includeProperties: "Employee,CreatedBy");

            return _mapper.Map<IEnumerable<EmployeeTaskViewModel>>(tasks);
        }

        public async Task<EmployeeTaskViewModel> GetTaskByIdAsync(int id)
        {
            var task = await _unitOfWork.EmployeeTasks
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == id,
                    includeProperties: "Employee,CreatedBy"
                );

            return _mapper.Map<EmployeeTaskViewModel>(task);
        }

        public async Task<IEnumerable<EmployeeTaskViewModel>> GetTasksByEmployeeIdAsync(int employeeId)
        {
            var tasks = await _unitOfWork.EmployeeTasks
                .GetAllAsync(
                    filter: t => t.EmployeeId == employeeId,
                    includeProperties: "Employee,CreatedBy"
                );

            return _mapper.Map<IEnumerable<EmployeeTaskViewModel>>(tasks);
        }

        public async Task<IEnumerable<EmployeeTaskViewModel>> GetTasksByManagerIdAsync(int managerId)
        {
            var tasks = await _unitOfWork.EmployeeTasks
                .GetAllAsync(
                    filter: t => t.CreatedById == managerId,
                    includeProperties: "Employee"
                );

            return _mapper.Map<IEnumerable<EmployeeTaskViewModel>>(tasks);
        }

        public async Task<EmployeeTaskViewModel> CreateTaskAsync(EmployeeTaskViewModel taskViewModel, int createdById)
        {
            var task = _mapper.Map<EmployeeTask>(taskViewModel);
            task.Status = Domain.Models.TaskStatus.New;
            task.CreatedById = createdById;
            
            await _unitOfWork.EmployeeTasks.AddAsync(task);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<EmployeeTaskViewModel>(task);
        }

        public async Task<EmployeeTaskViewModel> UpdateTaskStatusAsync(int taskId, Domain.Models.TaskStatus newStatus)
        {
            var task = await _unitOfWork.EmployeeTasks
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == taskId,
                    includeProperties: "Employee,CreatedBy"
                );
            
            if (task == null)
                return null;

            task.Status = newStatus;
            
            _unitOfWork.EmployeeTasks.Update(task);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<EmployeeTaskViewModel>(task);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.EmployeeTasks.GetByIdAsync(id);
            
            if (task == null)
                return false;

            _unitOfWork.EmployeeTasks.Remove(task);
            await _unitOfWork.CompleteAsync();
            
            return true;
        }

        public async Task<bool> TaskBelongsToEmployeeAsync(int taskId, int employeeId)
        {
            return await _unitOfWork.EmployeeTasks.AnyAsync(t => t.Id == taskId && t.EmployeeId == employeeId);
        }

        public async Task<EmployeeTaskViewModel> UpdateTaskAssignmentAsync(int taskId, int newEmployeeId)
        {
            var task = await _unitOfWork.EmployeeTasks
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == taskId,
                    includeProperties: "Employee,CreatedBy"
                );
            
            if (task == null)
                return null;

            task.EmployeeId = newEmployeeId;
            
            _unitOfWork.EmployeeTasks.Update(task);
            await _unitOfWork.CompleteAsync();
            
            // Refresh the task with updated employee info
            task = await _unitOfWork.EmployeeTasks
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == taskId,
                    includeProperties: "Employee,CreatedBy"
                );
                
            return _mapper.Map<EmployeeTaskViewModel>(task);
        }
    }
} 