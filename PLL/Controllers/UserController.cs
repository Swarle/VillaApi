using BusinessLogicLayer.Dto.User;
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
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetAllUsersAsync()
        {
            var response = await _userService.GetAllUsersAsync();

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-user/{id:guid}")]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetUserByIdAsync(Guid id)
        {
            var response = await _userService.GetUserByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-user")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> UpdateUserAsync([FromBody]UserUpdateDto updateDto)
        {
            var response = await _userService.UpdateUserAsync(updateDto);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> ChangePasswordAsync([FromBody]ChangePasswordDto changePasswordDto)
        {
            var response = await _userService.ChangePasswordAsync(changePasswordDto);

            return StatusCode((int)response.StatusCode, response);
        }


    }
}
