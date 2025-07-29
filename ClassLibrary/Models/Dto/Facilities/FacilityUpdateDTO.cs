#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class FacilityUpdateDTO
    {
        [Required]
        [MaxLength(10)]
        public string FacilityId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string LocationCoordinates { get; set; }

        public DateTime? OpeningHours { get; set; }
    }
}
