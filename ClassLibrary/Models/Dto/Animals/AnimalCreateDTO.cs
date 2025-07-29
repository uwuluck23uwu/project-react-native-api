#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class AnimalCreateDTO
    {
        [Required]
        public string HabitatId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Species { get; set; }

        [MaxLength(100)]
        public string ScientificName { get; set; }

        public string Description { get; set; }

        [MaxLength(100)]
        public string LocationCoordinates { get; set; }

        [MaxLength(1)]
        public string Status { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
