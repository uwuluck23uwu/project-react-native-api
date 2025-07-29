using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly ControllerHelper _controllerHelper;

        public AnimalController(IAnimalService animalService, IHostEnvironment hostEnvironment)
        {
            _animalService = animalService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnimals([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _animalService.GetAllAnimals(pageSize, currentPage, search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnimal([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _animalService.GetAsync(x => x.AnimalId == id));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAnimals([FromForm] AnimalCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _animalService.CreateAsync(createDTOs.Animals));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAnimal([FromForm] AnimalUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _animalService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAnimals([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(() => _animalService.DeleteImagesAsync(ids));
        }
    }
}
