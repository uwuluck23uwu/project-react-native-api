#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class OrderCreateDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public List<OrderItemCreateDTO> Items { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }
    }
}
