using App.Entity.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models.Property
{
    public class UserCredential
    {
        [Key]
        public long Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual AppUser? AppUser { get; set; }

        public string? Name { get; set; }
        public string? Path { get; set; }

        [StringLength(200)]
        public string? BlobName { get; set; }

    }
}
