#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class TicketCreateListDTO
    {
        [Required]
        public List<TicketCreateDTO> Tickets { get; set; }
    }
}
