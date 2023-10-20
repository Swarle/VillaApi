using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all-users")]
        [Authorize("AdminPolicy")]
        public async Task<ActionResult<ApiResponse>> GetAllUsersAsync()
        {
            var response = await _userService.GetAllUsersAsync();

            return StatusCode((int)response.StatusCode, response);
        }

    }
}
