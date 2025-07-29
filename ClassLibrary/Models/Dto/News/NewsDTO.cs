#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class NewsDTO
    {
        public string NewsId { get; set; }

        public string Title { get; set; }

        public string Contents { get; set; }

        public DateTime? PublishedDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<ImageDTO> Images { get; set; }
    }
}
