using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class PropertyFile
    {
        [Key]
        public long Id { get; set; }

        public long PropertyId {  get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        public string? ClientFileName { get; set; }
        public string? FileUrl { get; set; }

        [StringLength(100)]
        public string? FileType { get; set; }

        [StringLength(100)]
        public string? MimeType { get; set; }

        public string? BlobName { get; set; }
    }
}
