using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Application.DTOs
{
    public class ComparePasswordsAttribute : ValidationAttribute
    {
        private readonly string _passwordPropertyName;

        public ComparePasswordsAttribute(string passwordPropertyName)
        {
            _passwordPropertyName = passwordPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var passwordProperty = validationContext.ObjectType.GetProperty(_passwordPropertyName);

            if (passwordProperty == null)
            {
                return new ValidationResult($"Unknown property: {_passwordPropertyName}");
            }

            var passwordValue = passwordProperty.GetValue(validationContext.ObjectInstance) as string;
            var confirmedPasswordValue = value as string;

            if (passwordValue != confirmedPasswordValue)
                return new ValidationResult("The password and confirmed password do not match.");

            return ValidationResult.Success;
        }
    }
}
