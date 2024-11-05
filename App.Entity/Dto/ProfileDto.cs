using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class ProfileDto
    {
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.ValidationMessage_FirstName)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.ValidationMessage_LastName)]
        public string LastName { get; set; } = string.Empty;

        public string AgencyName { get; set; } = string.Empty;

        //[Required(ErrorMessage = ValidationMessges.ValidationMessage_CountryCode)]
        public string CountryCode { get; set; } = string.Empty;

        //[Required(ErrorMessage = ValidationMessges.ValidationMessage_PhoneNumber)]
        public string PhoneNumber { get; set; } = string.Empty;

        //[Required(ErrorMessage = ValidationMessges.ValidationMessage_CountryCode)]
        public int CountryId { get; set; }

        public string? Address { get; set; } = string.Empty;

        public string? Website { get; set; }
        public string? ProfileImage { get; set; }
        public double Lattitude { get; set; }
        public double Longitude { get; set; }
    }
}
