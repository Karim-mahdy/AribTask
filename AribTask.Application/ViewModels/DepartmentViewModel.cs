using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AribTask.Application.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
        public string Name { get; set; }
        
        public int EmployeeCount { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalSalary { get; set; }
    }
} 