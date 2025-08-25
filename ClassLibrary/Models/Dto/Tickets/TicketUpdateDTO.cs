#nullable disable
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class TicketUpdateDTO
    {
        [Required]
        [MaxLength(50)]
        public string TicketId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TicketType { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public List<string> ImageIds { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
