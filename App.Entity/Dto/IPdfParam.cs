using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Dto
{
    public interface IPdfParam
    {
        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public int PageNumber { get; set; }
        public double DX { get; set; }
        public double DY { get; set; }
        public double DW { get; set; }
        public double DH { get; set; }
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = ValidationMessges.Mandatory)]
        public string QR { get; set; }
    }
}
