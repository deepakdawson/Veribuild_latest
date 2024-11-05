using App.Entity.Validation;
using App.Foundation.Common;
using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class PropertyDto
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public double Lattitude { get; set; }

        [Required]
        public double Longitude { get; set; }


        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public double Area { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public int PropertyTypeId { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public int Bedroom { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? VimeoeUrl { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        [ValidateFileSize(Utils.MaxFloorPlanPdfSize)]
        public IFormFile? FloorPlan { get; set; }
        public IFormFile? Video { get; set; }
        public List<IFormFile> ImageFiles { get; set; } = [];
        public List<IFormFile> FloorPlanImageFiles { get; set; } = [];

        public string UserId { get; set; } = string.Empty;
        public string? VideoThumbnail { get; set; }
    }
}
