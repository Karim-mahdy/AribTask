using AribTask.Application.Common.Abstraction;
using AribTask.Application.Services.Interfaces;
using AribTask.Application.ViewModels;
using AribTask.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AribTask.Application.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment,UserManager<ApplicationUser> userManager ,IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees
                .GetAllAsync(includeProperties: "Department,Manager");

            return _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
        }

        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(filter: e => e.Id == id, includeProperties: "Department,Manager");

            return _mapper.Map<EmployeeViewModel>(employee);
        }

      public async Task<EmployeeViewModel> GetEmployeeByUserIdAsync(string userId)
        {
            // Find employee where InsertBy matches the userId
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(
                    filter: e => e.UserId == userId,
                    includeProperties: "Department,Manager"
                );

            return _mapper.Map<EmployeeViewModel>(employee);
        }
        public async Task<EmployeeViewModel> GetManagerEmployeeByUserIdAsync(string userId)
        {
            // Find employee where InsertBy matches the userId
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(
                    filter: e => e.InsertBy == userId,
                    includeProperties: "Department,Manager"
                );

            return _mapper.Map<EmployeeViewModel>(employee);
        }

        public async Task<IEnumerable<EmployeeViewModel>> SearchEmployeesAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllEmployeesAsync();

            var employees = await _unitOfWork.Employees
                .GetAllAsync(
                    filter: e => e.FirstName.Contains(searchTerm) || 
                           e.LastName.Contains(searchTerm) ||
                           e.Department.Name.Contains(searchTerm),
                    includeProperties: "Department,Manager"
                );

            return _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetEmployeesByManagerIdAsync(int managerId)
        {
            var employees = await _unitOfWork.Employees
                .GetAllAsync(
                    filter: e => e.ManagerId == managerId,
                    includeProperties: "Department"
                );

            return _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
        }
public async Task<EmployeeViewModel> CreateEmployeeAsync(EmployeeViewModel model, IFormFile imageFile)
{
    // Create employee
    var employee = _mapper.Map<Employee>(model);
    
    if (imageFile != null)
    {
        employee.ImagePath = await SaveImageAsync(imageFile);
    }
    
    // Create user account
    var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var user = new ApplicationUser
    {
        UserName = model.Email,
        Email = model.Email,
        FirstName = model.FirstName,
        LastName = model.LastName,
        EmailConfirmed = true
    };
    
    var result = await userManager.CreateAsync(user, "Employee@123"); // Default password
    if (result.Succeeded)
    {
      // Add to appropriate role based on position
        string role = "Employee";
        if (employee.ManagerId == null)
        {
            role = "Manager"; // If no manager, they are a manager
        }
        await userManager.AddToRoleAsync(user, role);
        
        // Link the employee to the user
        employee.InsertBy = user.Id;
        
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.CompleteAsync();
        
        return _mapper.Map<EmployeeViewModel>(employee);
    }
    
    throw new Exception("Failed to create user account");
}

        public async Task<EmployeeViewModel> UpdateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeViewModel.Id);
            
            if (employee == null)
                return null;

            var oldImagePath = employee.ImagePath;
            
            _mapper.Map(employeeViewModel, employee);
            
            if (imageFile != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    DeleteImage(oldImagePath);
                }
                
                employee.ImagePath = await SaveImageAsync(imageFile);
            }
            
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<EmployeeViewModel>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id,
                    includeProperties: "ManagedEmployees"
                );
            
            if (employee == null)
                return false;

            // Check if employee is a manager
            if (employee.ManagedEmployees.Any())
                return false;

            // Delete employee image if it exists
            if (!string.IsNullOrEmpty(employee.ImagePath))
            {
                DeleteImage(employee.ImagePath);
            }
            
            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();
            
            return true;
        }

        public async Task<bool> EmployeeIsManagerAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id,
                    includeProperties: "ManagedEmployees"
                );

            return employee?.ManagedEmployees?.Any() ?? false;
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _unitOfWork.Employees.AnyAsync(e => e.Id == id);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "employees");
            Directory.CreateDirectory(uploadsFolder);
            
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            
            return "/images/employees/" + uniqueFileName;
        }

        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            string fullPath = Path.Combine(_hostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
} 