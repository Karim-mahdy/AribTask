using AribTask.Application.Services.Interfaces;
using AribTask.Application.ViewModels;
using AribTask.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskStatus = AribTask.Domain.Models.TaskStatus;

namespace AribTask.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(
            ITaskService taskService, 
            IEmployeeService employeeService,
            UserManager<ApplicationUser> userManager)
        {
            _taskService = taskService;
            _employeeService = employeeService;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var applicationUser = await _userManager.FindByIdAsync(userId);

                // Find employee associated with current user
                var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                // var manager = await _employeeService.GetManagerEmployeeByUserIdAsync(userId);

                if (employee == null)
                {
                    return View(new List<EmployeeTaskViewModel>());
                }

                // If current user is a manager, get their employees
                if (User.IsInRole("Manager"))
                {
                    var managedEmployees = await _employeeService.GetEmployeesByManagerIdAsync(employee.Id);
                    ViewBag.Employees = managedEmployees.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.FullName
                    }).ToList();

                    // Get tasks ASSIGNED BY this manager (for managers)
                    var tasksAssignedByManager = await _taskService.GetTasksByManagerIdAsync(employee.Id);
                    ViewBag.IsManager = true;
                    return View(tasksAssignedByManager);
                }
                else if (employee != null)
                {
                    // Get tasks assigned TO this employee (for regular employees)
                    var tasks = await _taskService.GetTasksByEmployeeIdAsync(employee.Id);
                    ViewBag.IsManager = false;
                    return View(tasks);
                }
                else
                {
                    ViewBag.IsManager = false;
                    return View(new List<EmployeeTaskViewModel>());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = await _userManager.FindByIdAsync(userId);
            
            // Find employee associated with current user (manager)
            var manager = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (manager == null)
            {
                return NotFound("Manager profile not found");
            }

            // Get list of employees managed by this manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerIdAsync(manager.Id);
            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName");
            
            return View(new EmployeeTaskViewModel());
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(EmployeeTaskViewModel taskViewModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var manager = await _employeeService.GetEmployeeByUserIdAsync(userId);

                if (manager == null)
                    return Json(new { success = false, message = "Manager not found" });

              
                var managedEmployees = await _employeeService.GetEmployeesByManagerIdAsync(manager.Id);
                if (!managedEmployees.Any(e => e.Id == taskViewModel.EmployeeId))
                {
                    return Json(new { success = false, message = "Invalid employee assignment" });
                }

                var result = await _taskService.CreateTaskAsync(taskViewModel, manager.Id);
                return Json(new { success = true, message = "Task assigned successfully", redirectUrl = Url.Action(nameof(Index), nameof(TasksController)) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception", error = ex.Message });
            }
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var taskViewModel = await _taskService.GetTaskByIdAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (employee == null)
            {
                return NotFound("Employee profile not found");
            }

            // Verify that the task belongs to the current employee
            if (!await _taskService.TaskBelongsToEmployeeAsync(id, employee.Id))
            {
                return Forbid("You can only edit your own tasks");
            }

            ViewBag.StatusOptions = new SelectList(Enum.GetValues(typeof(TaskStatus)), taskViewModel.Status);
            return RedirectToAction(nameof(Index));
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskStatusUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, message = "ID mismatch" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee profile not found" });
            }

            // Verify that the task belongs to the current employee
            if (!await _taskService.TaskBelongsToEmployeeAsync(id, employee.Id))
            {
                return Json(new { success = false, message = "You can only edit your own tasks" });
            }

            if (ModelState.IsValid)
            {
                var updatedTask = await _taskService.UpdateTaskStatusAsync(id, model.Status);
                if (updatedTask == null)
                {
                    return Json(new { success = false, message = "Task not found" });
                }
                return Json(new { success = true, message = "Task updated successfully", task = updatedTask });
            }

            // Collect validation errors to return in the response
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { 
                    Key = x.Key, 
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList() 
                })
                .ToList();

            return Json(new { 
                success = false, 
                message = "Invalid model state", 
                validationErrors = errors,
                submittedData = new {
                    Id = model.Id,
                    Status = model.Status
                }
            });
        }

        // GET: Tasks/ManagerTasks
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ManagerTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (manager == null)
            {
                return NotFound("Manager profile not found");
            }

            // Get tasks created by this manager
            var tasks = await _taskService.GetTasksByManagerIdAsync(manager.Id);
            return View(tasks);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var taskViewModel = await _taskService.GetTaskByIdAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (employee == null)
            {
                return NotFound("Employee profile not found");
            }

            // Only allow viewing if user is either the assigned employee or the creator (manager)
            if (taskViewModel.EmployeeId != employee.Id && taskViewModel.CreatedById != employee.Id)
            {
                return Forbid("You don't have permission to view this task");
            }

            return View(taskViewModel);
        }
        
        // GET: Tasks/CreateModal
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateModal()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (manager == null)
            {
                return NotFound("Manager profile not found");
            }

            // Get list of employees managed by this manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerIdAsync(manager.Id);
            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName");
            
            return PartialView("_Create", new EmployeeTaskViewModel());
        }

        // GET: Tasks/EditModal/5
        public async Task<IActionResult> EditModal(int id)
        {
            var taskViewModel = await _taskService.GetTaskByIdAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (employee == null)
            {
                return NotFound("Employee profile not found");
            }

            // Verify that the task belongs to the current employee
            if (!await _taskService.TaskBelongsToEmployeeAsync(id, employee.Id))
            {
                return Forbid("You can only edit your own tasks");
            }

            ViewBag.StatusOptions = new SelectList(Enum.GetValues(typeof(TaskStatus)), taskViewModel.Status);
            return PartialView("_Edit", taskViewModel);
        }

        // GET: Tasks/DetailsModal/5
        public async Task<IActionResult> DetailsModal(int id)
        {
            var taskViewModel = await _taskService.GetTaskByIdAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (employee == null)
            {
                return NotFound("Employee profile not found");
            }

            // Only allow viewing if user is either the assigned employee or the creator (manager)
            if (taskViewModel.EmployeeId != employee.Id && taskViewModel.CreatedById != employee.Id)
            {
                return Forbid("You don't have permission to view this task");
            }

            return PartialView("_Details", taskViewModel);
        }

        // POST: Tasks/UpdateAssignment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateAssignment(int id, TaskAssignmentUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, message = "ID mismatch" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = await _employeeService.GetManagerEmployeeByUserIdAsync(userId);
                
            if (manager == null)
            {
                return Json(new { success = false, message = "Manager profile not found" });
            }

            // Verify that the task was created by this manager
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null || task.CreatedById != manager.Id)
            {
                return Json(new { success = false, message = "You can only reassign tasks you created" });
            }

            // Verify that the new employee is managed by this manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerIdAsync(manager.Id);
            if (!managedEmployees.Any(e => e.Id == model.EmployeeId))
            {
                return Json(new { success = false, message = "You can only assign tasks to employees you manage" });
            }

            if (ModelState.IsValid)
            {
                var updatedTask = await _taskService.UpdateTaskAssignmentAsync(id, model.EmployeeId);
                if (updatedTask == null)
                {
                    return Json(new { success = false, message = "Task not found" });
                }
                return Json(new { success = true, message = "Task reassigned successfully", task = updatedTask });
            }

            // Collect validation errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { 
                    Key = x.Key, 
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList() 
                })
                .ToList();

            return Json(new { 
                success = false, 
                message = "Invalid model state", 
                validationErrors = errors
            });
        }

        // GET: Tasks/AssignTaskModal/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignTaskModal(int id)
        {
            var taskViewModel = await _taskService.GetTaskByIdAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = await _employeeService.GetEmployeeByUserIdAsync(userId);
                
            if (manager == null)
            {
                return NotFound("Manager profile not found");
            }

            // Verify that the task was created by this manager
            if (taskViewModel.CreatedById != manager.Id)
            {
                return Forbid("You can only reassign tasks you created");
            }

            // Get list of employees managed by this manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerIdAsync(manager.Id);
            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName");
            
            return PartialView("_AssignTask", taskViewModel);
        }
    }
} 