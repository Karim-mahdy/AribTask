using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AribTask.Application.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Salary { get; set; }
        
        public string ImagePath { get; set; }
        
        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
        
        public string DepartmentName { get; set; }
        
        public int? ManagerId { get; set; }
        
        public string ManagerName { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
        
        // For image upload - not mapped to database
        public IFormFile ImageFile { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

    }
} 