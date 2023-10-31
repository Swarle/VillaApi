using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.Order;
using BusinessLogicLayer.Infrastructure;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse> GetOrdersAsync();
        Task<ApiResponse> GetOrderByIdAsync(Guid id);
        Task<ApiResponse> CreateOrderAsync(OrderCreateDto createDto);
    }
}
