#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class EventCreateListDTO
    {
        [Required]
        public List<EventCreateDTO> Events { get; set; }
    }
}
