using System;
using System.Collections.Generic;

namespace AribTask.Domain.Models
{
    public class Department : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Navigation properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
