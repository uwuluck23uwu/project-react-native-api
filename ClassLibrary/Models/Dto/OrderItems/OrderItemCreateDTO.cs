#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class OrderItemCreateDTO
    {
        [Required]
        public string RefId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal? PriceEach { get; set; }
    }
}
