using AribTask.Application.Services.Interfaces;
using AribTask.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AribTask.Controllers
{
    [Authorize(Roles ="Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Departments
        public async Task<IActionResult> Index(string searchString)
        {
            IEnumerable<DepartmentViewModel> departments;
            
            if (!string.IsNullOrEmpty(searchString))
            {
                departments = await _departmentService.SearchDepartmentsAsync(searchString);
            }
            else
            {
                departments = await _departmentService.GetAllDepartmentsAsync();
            }

            return View(departments);
        }

        // GET: Departments/Create
        public async Task<IActionResult> CreateModal()
        {
            return PartialView("_Create", new DepartmentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateModal(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.CreateDepartmentAsync(model);
                return Json(new { success = true });
            }
            return PartialView("_Create", model);
        }


        // GET: Departments/Edit/5
        public async Task<IActionResult> EditModal(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);
            if (model == null) return NotFound();
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditModal(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.UpdateDepartmentAsync(model);
                return Json(new { success = true });
            }

            return PartialView("_Edit", model);
        }

        public async Task<IActionResult> DeleteModal(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);
            if (model == null) return NotFound();
            return PartialView("_Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModal(DepartmentViewModel model)
        {
            var hasEmployees = await _departmentService.DepartmentHasEmployeesAsync(model.Id);
            if (hasEmployees)
            {
                ModelState.AddModelError("", "Cannot delete department with assigned employees.");
                return PartialView("_Delete", model);
            }

            await _departmentService.DeleteDepartmentAsync(model.Id);
            return Json(new { success = true });
        }

    }
} 