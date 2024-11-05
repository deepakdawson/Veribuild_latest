using System.ComponentModel.DataAnnotations;

namespace App.Entity.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? CustomerId { get; set; }

        public bool IsActive { get; set; }
        public int PropertyCredit { get; set; }
        public int UsedCredit { get; set; }
        public DateTime? BillingPeriod { get; set; }

        [StringLength(20)]
        public string? OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
