using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> LoginAsync([FromBody] LoginDto loginDto)
        {
            var response = await _userService.LoginAsync(loginDto);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> RegisterAsync([FromBody] RegistrationDto registerDto)
        {
            var response = await _userService.RegisterUserAsync(registerDto);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
