using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomValidations
{
    public class ValidDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Time is required.");
            }

            DateTime dateTime;
            var isValid = DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

            if (!isValid)
            {
                return new ValidationResult("Time must be a valid DateTime value.");
            }

            return ValidationResult.Success;
        }
    }
}
