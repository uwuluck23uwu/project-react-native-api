#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class PaymentDTO
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string Method { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public string ReferenceCode { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
