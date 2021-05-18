using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Core.Entities
{
    public class Activity
    {
        public int Id { get; set; }

        [Display(Name = "Activity Name")]
        [Required(ErrorMessage = "Activity Name is required")]
        [StringLength(100, ErrorMessage = "Activity Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Activity Type")]
        [Required]
        public ActivityType ActivityType { get; set; }

        [Remote(action: "VerifyStartDate", controller: "Activities", AdditionalFields = ("ModuleId,EndDate"))]
        [Display(Name = "StartDate")]
        [Required(ErrorMessage = "Activity StartDate is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Remote(action: "VerifyStartDate", controller: "Activities", AdditionalFields = ("ModuleId,StartDate"))]
        [Display(Name = "EndDate")]
        [Required(ErrorMessage = "Activity EndDate is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Activity Description is required")]
        [Display(Name = "Activity Description")]
        [StringLength(2000, ErrorMessage = "Activity Description cannot be longer than 2000 characters")]
        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public ICollection<Document> Documents { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> GetModulesSelectListItem { get; set; }
    }
}
