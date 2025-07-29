#nullable disable
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class ProductUpdateDTO
    {
        [Required]
        [MaxLength(10)]
        public string ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        [MaxLength(500)]
        public string QrCodeUrl { get; set; }

        public List<string> ImageIds { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
