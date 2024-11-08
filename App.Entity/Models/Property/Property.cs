using App.Entity.Models.Auth;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class Property
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual AppUser? User { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double Area { get; set; }
        public int PropertyTypeId { get; set; }

        [ForeignKey(nameof(PropertyTypeId))]
        public virtual PropertyType? PropertyType { get; set; }
        public int Bedroom { get; set; }
        public int EasyNumber { get; set; }
        public Point? LatLong { get; set; }
        public string? YoutubeUrls { get; set; }
        public string? VimeoUrls { get; set; }
        public string? VideoUrl { get; set; }
        
        public string? VideoThumb { get; set; }

        [StringLength(500)]
        public string FeatureImageUrl { get; set; } = string.Empty;
        public string? QrCode { get; set; }
        public string? QrLink { get; set; }
        public string UniqueId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<PropertyFile> PropertyFiles { get; set; } = [];
        public virtual ICollection<PropertyContract> PropertyContracts { get; set; } = [];
        public virtual ICollection<PropertyDocument> PropertyDocuments { get; set; } = [];

        [NotMapped]
        public string? BuilderName {  get; set; }

        [NotMapped]
        public string? UserPhoneNumber { get; set; }

    }
}
