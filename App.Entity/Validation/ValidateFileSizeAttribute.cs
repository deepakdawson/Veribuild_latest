using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Validation
{
    public class ValidateFileSizeAttribute(long maxSize) : ValidationAttribute
    {
        public long MaxSize { get; set; } = maxSize;


        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            IFormFile? file = value as IFormFile;
            if (file is not null && file.Length >= MaxSize)
            {
                return new ValidationResult($"{ErrorMessages.MaxSizeError}: {MaxSize / (1024 * 1024)}MB");
            }
            return ValidationResult.Success;

        }
    }
}
