using AribTask.Domain.Models;
using System.ComponentModel.DataAnnotations;
using TaskStatus = AribTask.Domain.Models.TaskStatus;

namespace AribTask.Application.ViewModels
{
    public class TaskStatusUpdateViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public TaskStatus Status { get; set; }
    }
} 