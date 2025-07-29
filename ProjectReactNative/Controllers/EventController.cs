using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ControllerHelper _controllerHelper;

        public EventController(IEventService eventService, IHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _eventService.GetAllEvents(pageSize, currentPage, search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _eventService.GetAsync(x => x.EventId == id));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvents([FromForm] EventCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _eventService.CreateAsync(createDTOs.Events));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateEvent([FromForm] EventUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _eventService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvents([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(() => _eventService.DeleteImagesAsync(ids));
        }
    }
}
