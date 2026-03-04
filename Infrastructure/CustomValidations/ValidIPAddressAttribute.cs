using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomValidations
{
    public class ValidIPAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("IP address is required.");
            }

            // Try to parse the IP address
            if (!IPAddress.TryParse(value.ToString(), out _))
            {
                return new ValidationResult("Invalid IP address.");
            }

            return ValidationResult.Success;
        }
    }
}
