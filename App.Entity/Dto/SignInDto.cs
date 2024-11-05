using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class SignInDto
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? PhoneCode { get; set; }
        public bool IsEmailLogin {  get; set; }

    }
}
