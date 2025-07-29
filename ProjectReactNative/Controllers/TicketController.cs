using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ControllerHelper _controllerHelper;

        public TicketController(ITicketService ticketService, IHostEnvironment hostEnvironment)
        {
            _ticketService = ticketService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _ticketService.GetAllTickets(pageSize, currentPage, search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _ticketService.GetAsync(x => x.TicketId == id));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateTickets([FromForm] TicketCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _ticketService.CreateAsync(createDTOs.Tickets));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateTicket([FromForm] TicketUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _ticketService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTickets([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(() => _ticketService.DeleteImagesAsync(ids));
        }
    }
}
