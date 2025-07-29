using Microsoft.AspNetCore.Mvc;

namespace ProjectReactNative.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        private readonly ControllerHelper _controllerHelper;

        public AuthenController(
            IAuthenService authenService,
            IHostEnvironment hostEnvironment)
        {
            _authenService = authenService;
            _controllerHelper = new ControllerHelper(hostEnvironment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            return await _controllerHelper.HandleRequest(() => _authenService.GetAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            return await _controllerHelper.HandleRequest(() => _authenService.Login(loginRequestDTO));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            return await _controllerHelper.HandleRequest(() => _authenService.Register(registerationRequestDTO));
        }

        [HttpPost]
        public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO)
        {
            return await _controllerHelper.HandleRequest(() => _authenService.RefreshAccessToken(tokenDTO));
        }
    }
}
