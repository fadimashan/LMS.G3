using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LMS.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<ApplicationUserCourse> AttendingCourses { get; set; }
        public ICollection<Course> Courses { get; set; }

        public ICollection<Document> Documents { get; set; }

    }
}
