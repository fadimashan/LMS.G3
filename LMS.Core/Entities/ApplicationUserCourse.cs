using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class ApplicationUserCourse
    {
        
        public int CourseId { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser Student { get; set; }

        public Course Course { get; set; }
    }
}
