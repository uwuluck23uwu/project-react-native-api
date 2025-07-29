#nullable disable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class NewsCreateListDTO
    {
        [Required]
        public List<NewsCreateDTO> News { get; set; }
    }
}
