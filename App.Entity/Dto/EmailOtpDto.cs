using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class EmailOtpDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string OTP { get; set; } = string.Empty;
    }
}
