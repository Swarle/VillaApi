using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto.Order;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Specification.OrderSpecification;
using DataLayer.UnitOfWork.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetOrderByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("Id is empty!");
                    return _response;
                }

                var specification = new FindOrderWithContactsAndVillasSpecification(id);

                var order = await _unitOfWork.Orders.FindSingle(specification);

                if (order == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Order with id:({id} not exist!)");
                    return _response;
                }

                var orderDto = _mapper.Map<OrderDto>(order);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = orderDto;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }

            return _response;
        }
    }
}
