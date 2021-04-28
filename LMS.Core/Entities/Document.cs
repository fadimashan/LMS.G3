using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class Document
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime UploadTime { get; set; }

        // I don't get it why should be the problem in here
        //public int ApplicationUserId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }


        // Navigation Properties
        public ApplicationUser ApplicationUser { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
        public Activity Activity { get; set; }
    }
}
