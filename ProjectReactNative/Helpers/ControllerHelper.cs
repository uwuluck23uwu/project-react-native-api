using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Helpers
{
    public class ControllerHelper
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly JsonResponse _encrypt;

        public ControllerHelper(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _encrypt = new JsonResponse();
        }

        public async Task<IActionResult> HandleRequest<T>(Func<Task<T>> action) where T : ResponseBase
        {
            try
            {
                var response = await action();
                return CreateResponse(response);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new ResponseErrorMessages(HttpStatusCode.BadRequest, false, e.Message));
            }
        }

        public IActionResult CreateResponse<T>(T response) where T : ResponseBase
        {
            return new ObjectResult(response) { StatusCode = (int)response.StatusCode };
        }
    }
}
