#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class RegisterationRequestDTO
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }
    }
}
