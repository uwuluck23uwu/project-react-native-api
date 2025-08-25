#nullable disable

using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class TicketDTO
    {
        public string TicketId { get; set; }

        public string TicketType { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<ImageDTO> Images { get; set; }
    }
}
