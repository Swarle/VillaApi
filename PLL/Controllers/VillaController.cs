using System.Net;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository.Interfaces;
using DataLayer.UnitOfWork.Interfaces;
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
        public async Task<ActionResult<ApiResponse>> GetVillasPartial()
        {
            var response = await _villaService.GetVillasPartialAsync();

            return StatusCode((int)response.StatusCode,response);
        }
        
        [HttpGet("get-villas")]
        public async Task<ActionResult<ApiResponse>> GetVillas()
        {
            var response = await _villaService.GetVillasAsync();

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-villa/{id:Guid}")]
        public async Task<ActionResult<ApiResponse>> GetVilla(Guid id)
        {
            var response = await _villaService.GetVillaByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("create-villa")]
        public async Task<ActionResult<ApiResponse>> CreateVillaAsync([FromBody] VillaCreateDto createDto)
        {
            var response = await _villaService.CreateVillaAsync(createDto);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-villa")]
        public async Task<ActionResult<ApiResponse>> UpdateVillaAsync([FromBody] VillaUpdateDto updateDto)
        {
            var response = await _villaService.UpdateVillaAsync(updateDto);

            return StatusCode((int)response.StatusCode, response);
        }



    }
}
