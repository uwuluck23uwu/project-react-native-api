#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class FacilityDTO
    {
        public string FacilityId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string LocationCoordinates { get; set; }

        public DateTime? OpeningHours { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
