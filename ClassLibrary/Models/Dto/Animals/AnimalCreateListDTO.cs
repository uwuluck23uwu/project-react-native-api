#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class AnimalCreateListDTO
    {
        [Required]
        public List<AnimalCreateDTO> Animals { get; set; }
    }
}
