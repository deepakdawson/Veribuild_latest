using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using App.Entity.Models.Property;

namespace App.Entity.Models.Auth
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string? AgencyName { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }

        public string? PhoneCode { get; set; }
        public int PhoneCodeId { get; set; }

        [ForeignKey(nameof(PhoneCodeId))]
        public virtual Country? Country { get; set; }

        public string? Profile { get;set; }

        public int OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization? Organization { get; set;}

        [StringLength(10)]
        public string? OTP { get; set; }
        public DateTime? OTPCreateTime { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<AppUserRoleMapping>? UserRoleMappings { get; set; }
        public virtual ICollection<UserCredential> UserCredentials { get; set; } = [];
    }
}
