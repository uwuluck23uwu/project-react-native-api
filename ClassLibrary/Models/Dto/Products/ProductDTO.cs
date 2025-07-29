#nullable disable

using ClassLibrary.Models.Data;

namespace ClassLibrary.Models.Dto
{
    public class ProductDTO
    {
        public string ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public int? StockQuantity { get; set; }

        public string QrCodeUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual QrScanLog QrScanLog { get; set; }

        public List<ImageDTO> Images { get; set; }
    }
}
