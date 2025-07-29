#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class LoginRequestDTO
    {
        [Required]
        [MaxLength(255)]
        public string Identifier { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
