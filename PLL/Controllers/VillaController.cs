using System.Net;
using System.Net.Mime;
using BusinessLogicLayer.Dto.Villa;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository.Interfaces;
using DataLayer.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly IVillaService _villaService;

        public VillaController(IVillaService villaService)
        {
            _villaService = villaService;
        }


        [HttpGet("get-villa-partial")]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status404NotFound,Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillasPartialAsync()
        {
            var response = await _villaService.GetVillasPartialAsync();

            return StatusCode((int)response.StatusCode,response);
        }

        [HttpGet("get-villas-statuses")]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillaStatusesAsync()
        {
            var response = await _villaService.GetVillaStatusesAsync();

            return StatusCode((int)response.StatusCode, response);
        }
        

        [HttpGet("get-villas")]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillasAsync()
        {
            var response = await _villaService.GetVillasAsync();

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-villa/{id:Guid}")]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillaAsync(Guid id)
        {
            var response = await _villaService.GetVillaByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("create-villa")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> CreateVillaAsync([FromForm] VillaCreateDto createDto)
        {
            var response = await _villaService.CreateVillaAsync(createDto);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-villa")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> UpdateVillaAsync([FromForm] VillaUpdateDto updateDto)
        {
            var response = await _villaService.UpdateVillaAsync(updateDto);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("delete-villa/{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse>> DeleteVillaAsync(Guid id)
        {
            var response = await _villaService.DeleteVillaAsync(id);

            return StatusCode((int)response.StatusCode,response);
        }


    }
}
