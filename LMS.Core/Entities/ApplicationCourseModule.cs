using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class ApplicationCourseModule
    {

        public int CourseId { get; set; }

        public int ModuleId { get; set; }

        public Module Module { get; set; }

        public Course Course { get; set; }
    }
}
