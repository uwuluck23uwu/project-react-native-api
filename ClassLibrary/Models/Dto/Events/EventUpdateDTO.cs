#nullable disable
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class EventUpdateDTO
    {
        [Required]
        [MaxLength(10)]
        public string EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        [MaxLength(100)]
        public string LocationCoordinates { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public DateTime? EventDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public List<string> ImageIds { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
