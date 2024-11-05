using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class PhoneDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory, AllowEmptyStrings = false)]
        public string PhoneCode { get; set; } = string.Empty;


        [Required(ErrorMessage = ValidationMessges.Mandatory, AllowEmptyStrings = false)]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Otp { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public int PhoneCodeId { get; set; }
    }
}
