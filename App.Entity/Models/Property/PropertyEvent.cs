using App.Entity.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class PropertyEvent
    {
        [Key]
        public long Id { get; set; }
        public long PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        [ForeignKey(nameof(CreatedBy))]
        public virtual AppUser? User { get; set; }

        public int? EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual EventType? Event { get; set; }

        public string? EventDescription { get; set; }
        public long? ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public virtual PropertyContract? PropertyContract { get; set; }

        public long? DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public virtual PropertyDocument? PropertyDocument { get; set; }

        public string? DocumentUrl { get; set; }
        public string RoleVisibility { get; set; } = "[]";
        public string? BlockchainUrl { get; set; }
        public string? TransectionId { get; set; }
        public string? VerifyUrl { get; set; }
        public string? ParentBlockchainUrl { get; set; }
        public string? Guid { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


    }
}
