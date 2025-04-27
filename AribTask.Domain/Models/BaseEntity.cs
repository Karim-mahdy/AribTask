using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribTask.Domain.Models
{
    public class BaseEntity : IBaseEntity
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? InsertBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? DeleteBy { get; set; }
        public bool? IsActive { get; set; } = true;
    }
    public interface IBaseEntity
    {
        public bool? IsDeleted { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? InsertBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? DeleteBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
