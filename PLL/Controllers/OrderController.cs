using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("get-villa/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> GetByIdAsync(Guid id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
