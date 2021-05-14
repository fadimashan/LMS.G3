using LMS.Core.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Validation
{
    class CheckUserName : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            const string errorMessage = "First Name and Last Name shouldn't be the same.";

            var user = (NewUserViewModel)validationContext.ObjectInstance;
            if (user.FirstName == user.LastName)
            {
                return new ValidationResult(errorMessage);
            }
            return ValidationResult.Success;



        }
    }
}
