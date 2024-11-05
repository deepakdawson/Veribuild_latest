using App.Entity.Models.Auth;
using App.Foundation.Messages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class UserInvite
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public long PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]    
        public virtual Property? Property { get; set; }

        public long? ContractId { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual AppUser? User { get; set; }

        [StringLength(450)]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(450)]
        public string InvitedBy { get; set; } = string.Empty;
        public DateTime CreateAt { get;set; }
        public DateTime UpdatedAt { get;set; }
    }
}
