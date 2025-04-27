using AribTask.Application.Common.Abstraction;
using AribTask.Application.Services.Interfaces;
using AribTask.Application.ViewModels;
using AribTask.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AribTask.Application.Services.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentViewModel>> GetAllDepartmentsAsync()
        {
            var departments = await _unitOfWork.Departments
                .GetAllAsync(includeProperties: "Employees");

            var departmentViewModels = departments.Select(d => 
            {
                var vm = _mapper.Map<DepartmentViewModel>(d);
                vm.EmployeeCount = d.Employees.Count;
                vm.TotalSalary = d.Employees.Sum(e => e.Salary);
                return vm;
            });

            return departmentViewModels;
        }

        public async Task<DepartmentViewModel> GetDepartmentByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments
                .GetFirstOrDefaultAsync(filter: d => d.Id == id, includeProperties: "Employees");

            if (department == null)
                return null;

            var departmentViewModel = _mapper.Map<DepartmentViewModel>(department);
            departmentViewModel.EmployeeCount = department.Employees.Count;
            departmentViewModel.TotalSalary = department.Employees.Sum(e => e.Salary);

            return departmentViewModel;
        }

        public async Task<IEnumerable<DepartmentViewModel>> SearchDepartmentsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllDepartmentsAsync();

            var departments = await _unitOfWork.Departments
                .GetAllAsync(
                    filter: d => d.Name.Contains(searchTerm),
                    includeProperties: "Employees"
                );

            var departmentViewModels = departments.Select(d => 
            {
                var vm = _mapper.Map<DepartmentViewModel>(d);
                vm.EmployeeCount = d.Employees.Count;
                vm.TotalSalary = d.Employees.Sum(e => e.Salary);
                return vm;
            });

            return departmentViewModels;
        }

        public async Task<DepartmentViewModel> CreateDepartmentAsync(DepartmentViewModel departmentViewModel)
        {
            var department = _mapper.Map<Department>(departmentViewModel);
            
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<DepartmentViewModel>(department);
        }

        public async Task<DepartmentViewModel> UpdateDepartmentAsync(DepartmentViewModel departmentViewModel)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentViewModel.Id);
            
            if (department == null)
                return null;

            _mapper.Map(departmentViewModel, department);
            
            _unitOfWork.Departments.Update(department);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<DepartmentViewModel>(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            
            if (department == null)
                return false;

            _unitOfWork.Departments.Remove(department);
            await _unitOfWork.CompleteAsync();
            
            return true;
        }

        public async Task<bool> DepartmentHasEmployeesAsync(int id)
        {
            var department = await _unitOfWork.Departments
                .GetFirstOrDefaultAsync(
                    filter: d => d.Id == id, 
                    includeProperties: "Employees"
                );

            return department?.Employees?.Any() ?? false;
        }
    }
} 