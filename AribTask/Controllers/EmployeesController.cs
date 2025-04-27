using AribTask.Application.Services.Interfaces;
using AribTask.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AribTask.Controllers
{
       [Authorize(Roles = "Admin,Manager")]
    
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService, IWebHostEnvironment hostEnvironment)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchResults = await _employeeService.SearchEmployeesAsync(searchString);
                return View(searchResults);
            }
            
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        // GET: Employees/Create

        [HttpGet]
        public async Task<IActionResult> CreateModal()
        {
            await PopulateViewBag();
            return PartialView("_Create", new EmployeeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateModal([FromForm] EmployeeViewModel model)
        {
           
                var saved = await _employeeService.CreateEmployeeAsync(model, model.ImageFile);

                return Json(new
                {
                    success = true,
                    employee = new
                    {
                        id = saved.Id,
                        firstName = saved.FirstName,
                        lastName = saved.LastName,
                        fullName = saved.FullName,
                        salary = saved.Salary.ToString("0.##"),
                        imagePath = saved.ImagePath,
                        departmentName = saved.DepartmentName,
                        managerName = string.IsNullOrEmpty(saved.ManagerName) ? "-" : saved.ManagerName
                    }
                });
            

            await PopulateViewBag(model.DepartmentId, model.ManagerId);
            return PartialView("_Create", model);
        }

        public async Task<IActionResult> GetEmployeeTableRows()
        {
            var employeeViewModels = await _employeeService.GetAllEmployeesAsync();
            return PartialView("_EmployeeTableRows", employeeViewModels);
        }
        // GET: Employees/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var model = await _employeeService.GetEmployeeByIdAsync(id);
            if (model == null) return NotFound();

            await PopulateViewBag(model.DepartmentId, model.ManagerId);
            return PartialView("_Edit", model);
        }


        [HttpPost]
        public async Task<IActionResult> EditModal([FromForm] EmployeeViewModel model)
        {
            
                var updated = await _employeeService.UpdateEmployeeAsync(model, model.ImageFile);

                return Json(new
                {
                    success = true,
                    employee = new
                    {
                        id = updated.Id,
                        firstName = updated.FirstName,
                        lastName = updated.LastName,
                        fullName = updated.FullName,
                        salary = updated.Salary.ToString("0.##"),
                        imagePath = updated.ImagePath,
                        departmentName = updated.DepartmentName,
                        managerName = string.IsNullOrEmpty(updated.ManagerName) ? "-" : updated.ManagerName
                    }
                });
            

            await PopulateViewBag(model.DepartmentId, model.ManagerId);
            return PartialView("_Edit", model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteModal(int id)
        {
            var model = await _employeeService.GetEmployeeByIdAsync(id);
            if (model == null) return NotFound();

            return PartialView("_Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModal(EmployeeViewModel model)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(model.Id);
            if (employee == null) return NotFound();

            if (!string.IsNullOrEmpty(employee.ImagePath))
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, employee.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            await _employeeService.DeleteEmployeeAsync(model.Id);
            return Json(new { success = true, id = model.Id });
        }


        private async Task PopulateViewBag(int? departmentId = null, int? managerId = null)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(departments, "Id", "Name", departmentId);
            
            var employees = await _employeeService.GetAllEmployeesAsync();
            if (managerId.HasValue)
            {
                // If editing, exclude the employee being edited to prevent self-assignment
                ViewBag.Managers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    employees, "Id", "FullName", managerId);
            }
            else
            {
                ViewBag.Managers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    employees, "Id", "FullName");
            }
        }
        //public async Task<IActionResult> GetEmployeeTableRows()
        //{
        //    var employees = await _employeeService.GetAllEmployeesAsync(); // must return List<Employee>
        //    return PartialView("_EmployeeTableRows", employees);
        //}

    }
} 