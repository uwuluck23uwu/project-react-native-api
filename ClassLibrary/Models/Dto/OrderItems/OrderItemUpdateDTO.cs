#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class OrderItemUpdateDTO
    {
        public string OrderItemId { get; set; }

        [Required]
        public string RefId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal? PriceEach { get; set; }
    }
}
