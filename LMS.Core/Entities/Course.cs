using Microsoft.AspNetCore.Mvc;
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
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Remote(action: "VerifyCourse", controller: "Courses")]
        [Required]
        [StringLength(100, ErrorMessage = "Course Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Course Description")]
        [StringLength(1000, ErrorMessage = "Course Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        [Display(Name = "StartDate")]
        [Required(ErrorMessage = "Course StartDate is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Remote(action: "VerifyCourseDate", controller: "Courses", AdditionalFields =("StartDate"))]
        [Display(Name = "EndDate")]
        [Required(ErrorMessage = "Course EndDate is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public ICollection<Module> Modules { get; set; }

        public ICollection<ApplicationUserCourse> Enrollments { get; set; }

        public ICollection<ApplicationUser> Students { get; set; }

        public ICollection<Document> Documents { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> GetModulesTitles { get; set; }
    }
}
