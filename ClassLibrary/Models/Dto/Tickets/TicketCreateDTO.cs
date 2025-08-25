#nullable disable
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class TicketCreateDTO
    {
        [Required]
        [MaxLength(50)]
        public string TicketType { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
