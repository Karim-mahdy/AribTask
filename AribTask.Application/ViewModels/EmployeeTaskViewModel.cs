using System;
using System.ComponentModel.DataAnnotations;
using AribTask.Domain.Models;
using TaskStatus = AribTask.Domain.Models.TaskStatus;

namespace AribTask.Application.ViewModels
{
    public class EmployeeTaskViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        
        public TaskStatus Status { get; set; }
        
        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeId { get; set; }
        
        public string EmployeeName { get; set; }
        
        public int CreatedById { get; set; }
        
        public string CreatedByName { get; set; }
        
        // Helper properties to determine styling
        public bool IsOverdue => DueDate.Date < DateTime.Today && Status != TaskStatus.Completed;
        public bool IsDueToday => DueDate.Date == DateTime.Today && Status != TaskStatus.Completed;
    }
} 