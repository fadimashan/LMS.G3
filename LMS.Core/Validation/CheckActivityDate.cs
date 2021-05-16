using LMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Validation
{
    class CheckActivityDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            const string errorMessage = "Activities must not overlap or go outside the module.";
            if (validationContext.ObjectInstance is Module module)
            {
                if (validationContext.ObjectInstance is Activity activity)
                {

                    if ((activity.StartDate >= module.StartDate && activity.EndDate <= module.EndDate))
                    {
                        return ValidationResult.Success;
                    }

                }
            }
            return new ValidationResult(errorMessage);
        }
    }
}
