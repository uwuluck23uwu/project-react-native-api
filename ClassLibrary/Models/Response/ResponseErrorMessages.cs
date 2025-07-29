using System.Net;

namespace ClassLibrary.Models.Response
{
    public class ResponseErrorMessages : ResponseBase
    {
        public List<ErrorModel> FieldError { get; set; } = new List<ErrorModel>();

        public ResponseErrorMessages(HttpStatusCode statusCode, bool taskStatus, string message)
            : base(statusCode, taskStatus, message) { }
    }

    public class ErrorModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
