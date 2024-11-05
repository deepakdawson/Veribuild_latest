using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public class DocumentDto : IPdfParam
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public long Property { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Label { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string UniqueId { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string Qrlink { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public double DX { get; set; }
        public double DY { get; set; }
        public double DW { get; set; }
        public double DH { get; set; }
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }
        public IFormFile? File { get; set; }
        public string QR { get; set; } = string.Empty;
    }
}
