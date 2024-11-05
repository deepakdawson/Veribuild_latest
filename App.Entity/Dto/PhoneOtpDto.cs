using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class PhoneOtpDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string PhoneCode { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string OTP { get; set; } = string.Empty;
    }
}
