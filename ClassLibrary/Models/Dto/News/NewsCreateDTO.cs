#nullable disable
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class NewsCreateDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Contents { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
