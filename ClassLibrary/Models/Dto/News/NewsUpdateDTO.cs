#nullable disable
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class NewsUpdateDTO
    {
        [Required]
        [MaxLength(10)]
        public string NewsId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Contents { get; set; }

        public List<string> ImageIds { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
