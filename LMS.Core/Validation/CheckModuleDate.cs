using LMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Validation
{
    class CheckModuleDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            const string errorMessage = "Modules must not overlap or go off course.";
            if (validationContext.ObjectInstance is Course course)
            {
                if (validationContext.ObjectInstance is Module module)
                {

                    if ((module.StartDate >= course.StartDate && module.EndDate <= course.EndDate))
                    {
                        return ValidationResult.Success;
                    }

                }
            }
            return new ValidationResult(errorMessage);
        }
    }
}
