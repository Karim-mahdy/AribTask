using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribTask.Domain.Models
{
    public class ApplicationUser  : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }
       
        public bool? IsDeleted { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? InsertBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? DeleteBy { get; set; }

    }
}
