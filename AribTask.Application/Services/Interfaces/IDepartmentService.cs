using AribTask.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AribTask.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentViewModel>> GetAllDepartmentsAsync();
        Task<DepartmentViewModel> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<DepartmentViewModel>> SearchDepartmentsAsync(string searchTerm);
        Task<DepartmentViewModel> CreateDepartmentAsync(DepartmentViewModel departmentViewModel);
        Task<DepartmentViewModel> UpdateDepartmentAsync(DepartmentViewModel departmentViewModel);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<bool> DepartmentHasEmployeesAsync(int id);
    }
} 