using AribTask.Application.Common.Abstraction;
using AribTask.Domain.Data;
using AribTask.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AribTask.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AribTaskDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            AribTaskDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            // Apply migrations
            await _context.Database.MigrateAsync();
            
            // Seed roles
            await SeedRolesAsync();
             

            // Seed users
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                
            if (!await _roleManager.RoleExistsAsync("Manager"))
                await _roleManager.CreateAsync(new IdentityRole("Manager"));
                
            if (!await _roleManager.RoleExistsAsync("Employee"))
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
        }
         
        private async Task SeedUsersAsync()
        {
            try
            {
                if (await _userManager.FindByNameAsync("admin") == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User"
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Create manager user + employee record
                ApplicationUser managerUser = await _userManager.FindByNameAsync("manager");
                if (managerUser == null)
                {
                    managerUser = new ApplicationUser
                    {
                        UserName = "manager",
                        Email = "manager@example.com",
                        EmailConfirmed = true,
                        FirstName = "Manager",
                        LastName = "User"
                    };

                    var result = await _userManager.CreateAsync(managerUser, "Manager@123");
                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(managerUser, "Manager");
                }

                Department department = null;
                if (!_context.Departments.Any())
                {
                    department = new Department { Name = "IT" };
                   var result = _context.Departments.Add(department);
                    await _context.SaveChangesAsync();
                }

                if (department == null || department.Id == 0)
                {
                    department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == "IT");
                }

                // Create Manager Employee Record
                if (!_context.Employees.Any(e => e.UserId == managerUser.Id))
                {
                    var managerEmployee = new Employee
                    {
                        FirstName = "Manager",
                        LastName = "User",
                        Salary = 15000, // Example salary
                        DepartmentId =department.Id, // Example department
                        IsActive = true,
                        InsertDate = DateTime.UtcNow,
                        UserId = managerUser.Id,
                    };
                    _context.Employees.Add(managerEmployee);
                    await _context.SaveChangesAsync();
                }

                // Get manager employee to link employees
                var managerEmployeeEntity = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == managerUser.Id);

                // Create 3 employee users + employee records
                for (int i = 1; i <= 3; i++)
                {
                    string username = $"employee{i}";
                    if (await _userManager.FindByNameAsync(username) == null)
                    {
                        var employeeUser = new ApplicationUser
                        {
                            UserName = username,
                            Email = $"employee{i}@example.com",
                            EmailConfirmed = true,
                            FirstName = $"Employee{i}",
                            LastName = "User"
                        };

                        var result = await _userManager.CreateAsync(employeeUser, "Employee@123");
                        if (result.Succeeded)
                            await _userManager.AddToRoleAsync(employeeUser, "Employee");

                        // Create Employee entity
                        var employeeEntity = new Employee
                        {
                            FirstName = $"Employee{i}",
                            LastName = "User",
                            Salary = 8000 + i * 100, // Example salary
                            DepartmentId = department.Id, // Example department
                            ManagerId = managerEmployeeEntity.Id, // Link to manager
                            IsActive = true,
                            InsertDate = DateTime.UtcNow,
                            UserId = employeeUser.Id,
                            InsertBy = managerEmployeeEntity.UserId
                        };
                        _context.Employees.Add(employeeEntity);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex ?? new Exception("An error occurred while seeding the database.");
            }
          
        }

    }
}