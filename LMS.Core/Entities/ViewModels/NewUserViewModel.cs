using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Core.Entities.ViewModels
{
    public class NewUserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public string RoleType { get; set; }
        public string Password { get; set; }

        public int CourseId { get; set; }

        public IEnumerable<SelectListItem> GetAllCourses { get; set; }
    }
}
