using System.ComponentModel.DataAnnotations;

namespace App.Entity.Models
{
    public class Country
    {
        [Key]
        public int Id { get;set; }

        [StringLength(50)]
        public string ISO { get; set; } = string.Empty;

        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string NickName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ISO3 { get; set; }
        public short? NumCode { get; set; }
        public short? PhoneCode { get; set; }
    }
}
