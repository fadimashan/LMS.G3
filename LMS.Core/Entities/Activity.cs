using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Core.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public ICollection<Document> Documents { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> GetModulesSelectListItem { get; set; }
    }
}
