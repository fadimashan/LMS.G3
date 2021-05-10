using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class Module
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }


        [NotMapped]
        public IEnumerable<SelectListItem> GetAllCourses { get; set; }

        public ICollection<Activity> Activities { get; set; }

        public ICollection<Document> Documents { get; set; }



    }
}
