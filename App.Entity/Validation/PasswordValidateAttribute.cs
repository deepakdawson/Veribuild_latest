using App.Foundation.Common;
using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Validation
{
    public class PasswordValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string password = value as string ?? "";
            bool isvalid = PasswordUtil.ValidatePassword(password);
            if (isvalid)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessages.PasswordValidationError);
        }
    }
}
