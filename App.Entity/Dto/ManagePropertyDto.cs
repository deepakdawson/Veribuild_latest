using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class ManagePropertyDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public long Property { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public long Contract { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public int EventId { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public IFormFile? File { get; set; }

        public string RoleVisibility { get; set; } = "[]";
        public string UserId = string.Empty;
    }
}
