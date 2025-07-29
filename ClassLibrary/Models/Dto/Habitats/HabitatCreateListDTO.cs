#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class HabitatCreateListDTO
    {
        [Required]
        public List<HabitatCreateDTO> Habitats { get; set; }
    }
}
