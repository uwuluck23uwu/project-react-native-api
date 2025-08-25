#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class LocationCreateDTO
    {
        [MaxLength(10)]
        public string RefId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string X { get; set; }

        [MaxLength(10)]
        public string Y { get; set; }

        public string Description { get; set; }

        public string Activities { get; set; }
    }
}
