using App.Entity.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class PropertyDocument
    {
        [Key]
        public long Id { get; set; }

        public long PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]    
        public virtual Property? Property { get; set; }

        public string? CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual AppUser? CreatedByUser { get; set; }


        public DateTime Date { get; set; }

        public string? Label { get; set; }
        public string? Url { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
        public string QrLink { get; set; } = string.Empty;
        public string? BlockchainUrl { get; set; }
        public string? TransectionId { get; set; }
        public string? FileHash { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
