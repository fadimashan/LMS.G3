using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Core.Entities.ViewModels
{
    public class NewUserViewModel
    {
        [CheckUserName]
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First Name must be between 3 to 50 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 to 50 characters")]
        public string LastName { get; set; }

        public string RoleType { get; set; }

        //[CheckUserEmail]
        [Remote(action: "VerifyEmail", controller: "Courses")]
        [Display(Name ="Email address")]
        [Required(ErrorMessage ="The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "The password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirmed Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Required(ErrorMessage = "The confirmed password is required")]
        [NotMapped]
        public string ConfirmedPassword { get; set; }

        public int CourseId { get; set; }
        public IEnumerable<SelectListItem> GetAllCourses { get; set; }
    }
}
