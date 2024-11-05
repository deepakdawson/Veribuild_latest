using App.Entity.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class PropertyContract
    {
        [Key] public long Id { get; set; }

        public long PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        public string? CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual AppUser? CreatedByUser { get; set; }

        public string Number { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        [StringLength(300)]
        public string DocumentUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
        public string QrLink { get; set; } = string.Empty;
        public string? Guid { get; set; }
        public string? BlockchainUrl { get; set; }
        public string? TransectionId { get; set; }
        public string? FileHash { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
