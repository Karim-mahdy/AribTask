using System;

namespace AribTask.Domain.Models
{
    public class EmployeeTask : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        
        // Employee assigned to this task
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        
        // Creator of the task (manager)
        public int CreatedById { get; set; }
        public virtual Employee CreatedBy { get; set; }
    }
    
    public enum TaskStatus
    {
        New,
        InProgress,
        Completed,
        Delayed,
        Cancelled
    }
} 