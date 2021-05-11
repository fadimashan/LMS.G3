using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Core.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<Module> Modules { get; set; }
        
        public ICollection<Document> Documents { get; set; }
        
        public ICollection<ApplicationUser> Students { get; set; }

        public ICollection<ApplicationUserCourse> Enrollments { get; set; }
        
        [NotMapped]
        public IEnumerable<SelectListItem> GetModulesTitles { get; set; }
    }
}
