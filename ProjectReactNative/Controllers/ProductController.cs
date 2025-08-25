using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ControllerHelper _controllerHelper;

        public ProductController(IProductService productService, IHostEnvironment hostEnvironment)
        {
            _productService = productService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _productService.GetAllProducts(pageSize, currentPage, search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _productService.GetAsync(x => x.ProductId == id));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateListDTO createDTOs)
        {
            return await _controllerHelper.HandleRequest(() => _productService.CreateAsync(createDTOs.Products));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateDTO updateDTO)
        {
            return await _controllerHelper.HandleRequest(() => _productService.UpdateAsync(updateDTO));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromBody] IEnumerable<string> ids)
        {
            return await _controllerHelper.HandleRequest(
                async () =>
                {
                    await _productService.DeleteLocationAsync(ids);
                    return await _productService.DeleteImagesAsync(ids);
                }
            );
        }
    }
}
