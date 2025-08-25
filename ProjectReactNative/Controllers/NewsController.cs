using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ControllerHelper _controllerHelper;

        public NewsController(INewsService NewsService, IHostEnvironment hostEnvironment)
        {
            _newsService = NewsService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNews([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _newsService.GetAllNews(pageSize, currentPage, search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNews([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _newsService.GetAsync(x => x.NewsId == id));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateNews([FromForm] NewsCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _newsService.CreateAsync(createDTOs.News));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateNews([FromForm] NewsUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _newsService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(
                async () =>
                {
                    await _newsService.DeleteLocationAsync(ids);
                    return await _newsService.DeleteImagesAsync(ids);
                }
            );
        }
    }
}