using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ControllerHelper _controllerHelper;

        public LocationController(ILocationService locationService, IHostEnvironment hostEnvironment)
        {
            _locationService = locationService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _locationService.GetAllLocations(pageSize, currentPage, search));
        }
    }
}
