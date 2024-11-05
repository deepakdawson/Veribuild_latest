using App.Foundation.Common;
using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Validation
{
    public class PasswordValidateNullAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? password = value as string;
            if (string.IsNullOrEmpty(password)) { return ValidationResult.Success; }
            bool isvalid = PasswordUtil.ValidatePassword(password);
            if (isvalid)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessages.PasswordValidationError);
        }
    }
}
