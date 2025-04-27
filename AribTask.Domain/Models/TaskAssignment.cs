namespace AribTask.Domain.Models
{
    public class TaskAssignment : BaseEntity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // e.g. New, InProgress, Done
        public int CreatedByManagerId { get; set; }
    }

}
