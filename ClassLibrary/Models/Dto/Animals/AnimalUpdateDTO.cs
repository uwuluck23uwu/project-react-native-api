#nullable disable
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class AnimalUpdateDTO
    {
        [Required]
        [MaxLength(10)]
        public string AnimalId { get; set; }

        [Required]
        public string HabitatId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Species { get; set; }

        [MaxLength(150)]
        public string ScientificName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public LocationUpdateDTO Location { get; set; }

        public List<string> ImageIds { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
