using App.Entity.Validation;
using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class PasswordDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        [PasswordValidate]
        public string NewPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        [Compare(nameof(NewPassword), ErrorMessage = ValidationMessges.PasswordMismatch)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
