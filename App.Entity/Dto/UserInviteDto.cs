using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class UserInviteDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public long Property { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public long Contract { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public int PhoneCodeId { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string PhoneCode { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string PhoneNumber { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public int OrganisationId { get; set; }
    }
}
