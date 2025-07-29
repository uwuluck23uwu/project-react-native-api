using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _facilitieService;
        private readonly ControllerHelper _controllerHelper;

        public FacilityController(IFacilityService facilitieService, IHostEnvironment hostEnvironment)
        {
            _facilitieService = facilitieService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFacilities([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            Expression<Func<Facility, bool>> filter = x => string.IsNullOrEmpty(search) || x.Name.Contains(search);
            return await _controllerHelper.HandleRequest(() => _facilitieService.GetAllAsync(pageSize, currentPage, search, null));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFacilitie([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _facilitieService.GetAsync(x => x.FacilityId == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFacilitie([FromBody] FacilityCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _facilitieService.CreateAsync(createDTOs.Facilities));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFacilitie([FromBody] FacilityUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _facilitieService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFacilitie([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(() => _facilitieService.DeleteAsync(ids));
        }
    }
}
