#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class FacilityCreateListDTO
    {
        [Required]
        public List<FacilityCreateDTO> Facilities { get; set; }
    }
}
