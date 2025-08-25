#nullable disable
using ClassLibrary.Models.Data;

namespace ClassLibrary.Models.Dto
{
    public class OrderDTO
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public int? TotalAmount { get; set; }
        public decimal? TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? OrderDatetime { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
