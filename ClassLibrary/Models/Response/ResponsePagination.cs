using System.Net;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Response
{
    public class ResponsePagination : ResponseBase
    {
        [JsonPropertyOrder(1)]
        public object Pagin { get; set; }
        [JsonPropertyOrder(2)]
        public List<object> Data { get; set; }
        public ResponsePagination(HttpStatusCode statusCode, bool taskStatus, string message, object pagin, List<object> data)
            : base(statusCode, taskStatus, message)
        {
            Pagin = pagin;
            Data = data;
        }
    }
}
