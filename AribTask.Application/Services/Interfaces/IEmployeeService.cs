using AribTask.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AribTask.Application.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync();
        Task<EmployeeViewModel> GetEmployeeByIdAsync(int id);
        Task<EmployeeViewModel> GetEmployeeByUserIdAsync(string userId);
        Task<EmployeeViewModel> GetManagerEmployeeByUserIdAsync(string userId);
        Task<IEnumerable<EmployeeViewModel>> SearchEmployeesAsync(string searchTerm);
        Task<IEnumerable<EmployeeViewModel>> GetEmployeesByManagerIdAsync(int managerId);
        Task<EmployeeViewModel> CreateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile);
        Task<EmployeeViewModel> UpdateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> EmployeeIsManagerAsync(int id);
        Task<bool> EmployeeExistsAsync(int id);
    }
} 