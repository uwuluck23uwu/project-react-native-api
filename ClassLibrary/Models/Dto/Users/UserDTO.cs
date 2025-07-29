#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class UserDTO
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string ImageUrl { get; set; }

        public string Role { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
