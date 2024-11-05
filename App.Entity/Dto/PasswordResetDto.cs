using App.Entity.Validation;
using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class PasswordResetDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [PasswordValidate]
        [MinLength(8, ErrorMessage = "Minimum 8 characters are required.")]
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = ValidationMessges.PasswordMismatch)]
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
