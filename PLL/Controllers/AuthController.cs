using System.Text;
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
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> LoginAsync([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> RegisterAsync([FromBody] RegistrationDto registerDto)
        {
            var response = await _authService.RegisterUserAsync(registerDto);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
