using BusinessLogicLayer.Dto.Order;
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

        [HttpGet("get-orders")]
        public async Task<ActionResult<ApiResponse>> GetOrdersAsync()
        {
            var response = await _orderService.GetOrdersAsync();

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpGet("get-order/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> GetByIdAsync(Guid id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("make-order")]
        public async Task<ActionResult<ApiResponse>> MakeOrderAsync(OrderCreateDto createDto)
        {
            var response = await _orderService.CreateOrderAsync(createDto);

            return StatusCode((int)response.StatusCode,response);
        }

        [HttpPut("update-order")]
        public async Task<ActionResult<ApiResponse>> UpdateOrderAsync(OrderUpdateDto updateDto)
        {
            var response = await _orderService.UpdateOrderAsync(updateDto);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
