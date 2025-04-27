using System.ComponentModel.DataAnnotations;

namespace AribTask.Application.ViewModels
{
    public class TaskAssignmentUpdateViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeId { get; set; }
    }
} 