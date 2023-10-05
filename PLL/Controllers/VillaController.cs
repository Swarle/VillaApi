using System.Net;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository.Interfaces;
using DataLayer.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<VillaController>
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetVillas()
        {
            var response = await _villaService.GetVillasPartialAsync();

            return StatusCode((int)response.StatusCode,response);
        }

        
    }
}
