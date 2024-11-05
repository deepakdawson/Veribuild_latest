using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    [Index(nameof(ImageType))]
    public class PropertyImage
    {
        [Key]
        public long Id { get; set; }

        public long PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [StringLength(500)]
        public string? ClientName { get; set; }

        [StringLength(500)]
        public string? Url { get; set; }

        [StringLength(100)]
        public string ImageType { get; set; } = string.Empty;

        [StringLength(200)]
        public string BlobName { get; set; } = string.Empty;
    }
}
