#nullable disable
using ClassLibrary.Models.Data;

namespace ClassLibrary.Models.Dto
{
    public class AnimalDTO
    {
        public string AnimalId { get; set; }

        public string HabitatId { get; set; }

        public string Name { get; set; }

        public string Species { get; set; }

        public string ScientificName { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Habitat Habitat { get; set; }

        public LocationDTO Location { get; set; }

        public List<ImageDTO> Images { get; set; }
    }
}
