using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace AribTask.Domain.Models
{
    public class Employee : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Salary { get; set; }
        public string? ImagePath { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        // Department relationship
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        
        // Manager relationship (self-referencing)
        public int? ManagerId { get; set; }
        public virtual Employee Manager { get; set; }
        public virtual ICollection<Employee> ManagedEmployees { get; set; } = new List<Employee>();
        
        // Tasks assigned to this employee
        public virtual ICollection<EmployeeTask> AssignedTasks { get; set; } = new List<EmployeeTask>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped] 
        public IFormFile ImageFile { get; set; }
    }
}
