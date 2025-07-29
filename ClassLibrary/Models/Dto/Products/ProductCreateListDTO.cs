#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class ProductCreateListDTO
    {
        [Required]
        public List<ProductCreateDTO> Products { get; set; }
    }
}
