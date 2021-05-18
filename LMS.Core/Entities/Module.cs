using LMS.Core.Validation;
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
    public class Module
    {
        public int Id { get; set; }

        [Display(Name = "Module Title")]
        [Required(ErrorMessage = "Module Title is required")]
        [StringLength(100, ErrorMessage = "Module Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Module Description")]
        [StringLength(1000, ErrorMessage = "Module Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        [Remote(action: "VerifyModuleStartDate", controller: "Modules", AdditionalFields = ("CourseId,EndDate"))]
        [Display(Name = "StartDate")]
        [Required(ErrorMessage = "Module StartDate is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Remote(action: "VerifyModuleStartDate", controller: "Modules", AdditionalFields = ("CourseId,StartDate"))]
        [Display(Name = "EndDate")]
        [Required(ErrorMessage = "Module EndDate is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }


        [NotMapped]
        public IEnumerable<SelectListItem> GetAllCourses { get; set; }

        public ICollection<Activity> Activities { get; set; }

        public ICollection<Document> Documents { get; set; }



    }
}
