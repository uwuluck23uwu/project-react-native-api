namespace ClassLibrary.Models.Response
{
    public class ResponsePagin
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
    }
}
