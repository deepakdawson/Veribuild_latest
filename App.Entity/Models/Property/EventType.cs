using App.Entity.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class EventType
    {
        public int Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? NormalizedName { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [ForeignKey((nameof(CreatedBy)))]
        public virtual AppUser? User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
