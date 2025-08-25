#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class EventDTO
    {
        public string EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime? EventDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public LocationDTO Location { get; set; }

        public List<ImageDTO> Images { get; set; }
    }
}
