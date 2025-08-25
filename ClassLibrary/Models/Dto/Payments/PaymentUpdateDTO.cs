#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class PaymentUpdateDTO
    {
        [Required]
        public string PaymentId { get; set; }

        public string Status { get; set; }
        public string ReferenceCode { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
