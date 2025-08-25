#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class OrderItemDTO
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
    }
}
