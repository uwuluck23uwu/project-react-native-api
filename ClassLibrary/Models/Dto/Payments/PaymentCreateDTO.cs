#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class PaymentCreateDTO
    {
        [Required]
        public string OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Method { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
