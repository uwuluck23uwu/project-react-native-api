using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HabitatController : ControllerBase
    {
        private readonly IHabitatService _habitatService;
        private readonly ControllerHelper _controllerHelper;

        public HabitatController(IHabitatService habitatService, IHostEnvironment hostEnvironment)
        {
            _habitatService = habitatService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHabitats([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _habitatService.GetAllAsync(pageSize, currentPage, search, null));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHabitat([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _habitatService.GetAsync(x => x.HabitatId == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateHabitats([FromBody] HabitatCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _habitatService.CreateAsync(createDTOs.Habitats));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHabitat([FromBody] HabitatUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _habitatService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHabitats([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(() => _habitatService.DeleteAsync(ids));
        }
    }
}
