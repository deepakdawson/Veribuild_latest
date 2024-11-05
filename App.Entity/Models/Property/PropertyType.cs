using System.ComponentModel.DataAnnotations;

namespace App.Entity.Models.Property
{
    public class PropertyType
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
