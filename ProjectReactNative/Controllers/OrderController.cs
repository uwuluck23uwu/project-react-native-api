using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ControllerHelper _controllerHelper;

        public OrderController(IOrderService orderService, IHostEnvironment hostEnvironment)
        {
            _orderService = orderService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders([Required] int pageSize = 10, [Required] int currentPage = 1, string search = "")
        {
            return await _controllerHelper.HandleRequest(() => _orderService.GetAllOrders(pageSize, currentPage, search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([Required] string id)
        {
            return await _controllerHelper.HandleRequest(() => _orderService.GetAsync(x => x.OrderId == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO dto)
        {
            return await _controllerHelper.HandleRequest(() => _orderService.CreateOrderAndPaymentAsync(dto));
        }

        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/json")]
        public async Task<IActionResult> StripeWebhook()
        {
            return await _controllerHelper.HandleRequest(() => _orderService.HandleStripeWebhookAsync(Request));
        }
    }
}
