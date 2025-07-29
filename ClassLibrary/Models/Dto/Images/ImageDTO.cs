#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class ImageDTO
    {
        public string ImageId { get; set; }

        public string RefId { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? UploadedDate { get; set; }
    }
}
