using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto.Order;
using BusinessLogicLayer.Infastructure;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Models;
using DataLayer.Specification.OrderSpecification;
using DataLayer.Specification.OrderStatusSpecification;
using DataLayer.Specification.UserSpecification;
using DataLayer.Specification.VillaSpecification;
using DataLayer.Specification.VillaStatusSpecification;
using DataLayer.UnitOfWork.Interfaces;
using Utility;

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

        public async Task<ApiResponse> GetOrdersAsync()
        {
            try
            {
                var specification = new FindOrderWithContactsAndVillasSpecification();

                var orders = await _unitOfWork.Orders.Find(specification);

                if (!orders.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("Orders do not exist!");
                    return _response;
                }

                var ordersDto = _mapper.Map<List<OrderPartialDto>>(orders);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = ordersDto;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }

            return _response;
        }

        public async Task<ApiResponse> GetOrderStatusesAsync()
        {
            try
            {
                var orderStatuses = await _unitOfWork.OrderStatus.GetAllAsync();

                if (!orderStatuses.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("No status was found!");
                    return _response;
                }

                var orderStatusesDto = _mapper.Map<List<OrderStatusDto>>(orderStatuses);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = orderStatusesDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = ex.FromHierarchy(e => e.InnerException).Select(e => e.Message).ToList();
            }
            return _response;
        }

        public async Task<ApiResponse> GetOrdersByUserIdAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("Id field is empty!");
                    return _response;
                }

                var orderSpecification = new FindOrderByUserIdSpecification(userId);

                var orders = await _unitOfWork.Orders.Find(orderSpecification);

                if (!orders.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"No orders found for user with ID ({userId})");
                    return _response;
                }

                var orderDto = _mapper.Map<List<OrderDto>>(orders);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = orderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = ex.FromHierarchy(e => e.InnerException).Select(e => e.Message).ToList();
            }

            return _response;
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

        public async Task<ApiResponse> CreateOrderAsync(OrderCreateDto createDto)
        {
            try
            {
                if (!IsValidOrderDate(createDto.CheckIn, createDto.CheckOut))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("The CheckIn or CheckOut fields did not pass validation!");
                    return _response;
                }

                var findUserSpecification = new FindUserByIdSpecification(createDto.UserId);

                var isUserExist = await _unitOfWork.Users.FindAny(findUserSpecification);

                if (!isUserExist)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"User with id:({createDto.UserId}) do not exist!");
                    return _response;
                }

                var orderStatusSpecification = new FindOrderStatusSpecification(OrderStatusSD.InProgress);

                var status = await _unitOfWork.OrderStatus.FindSingle(orderStatusSpecification);

                if (status == null)
                {
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessage.Add($"Status {OrderStatusSD.InProgress} was not found!");
                    return _response;
                }

                createDto.StatusId = status.Id;

                var villaSpecification = new FindVillaSpecification(createDto.VillaId);

                var isVillaExist = await _unitOfWork.Villas.FindAny(villaSpecification);

                if (!isVillaExist)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Villa with ID ({createDto.VillaId}) does not exist");
                    return _response;
                }

                var villaOccupiedSpecification =
                    new IsVillaOccupied(createDto.VillaId, createDto.CheckIn, createDto.CheckOut);

                var isVillaOccupied = await _unitOfWork.Villas.FindAny(villaOccupiedSpecification);

                if (isVillaOccupied)
                {
                    _response.StatusCode = HttpStatusCode.Conflict;
                    _response.ErrorMessage.Add("The villa is already booked for that time!");
                    return _response;
                }
                
                var order = _mapper.Map<Orders>(createDto);

                await _unitOfWork.Orders.CreateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var orderSpecification = new FindOrderWithContactsAndVillasSpecification(order.Id);

                var foundOrder = await _unitOfWork.Orders.FindSingle(orderSpecification);

                var orderDto = _mapper.Map<OrderDto>(foundOrder);

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

        public async Task<ApiResponse> UpdateOrderAsync(OrderUpdateDto updateDto)
        {
            try
            {
                if (!IsValidOrderDate(updateDto.CheckIn, updateDto.CheckOut))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("The CheckIn or CheckOut fields did not pass validation!");
                    return _response;
                }

                var orderSpecification = new FindOrderWithContactsAndVillasSpecification(updateDto.Id);

                var order = await _unitOfWork.Orders.FindSingle(orderSpecification);

                if (order == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Order with ID {updateDto.Id} already exists");
                    return _response;
                }

                if (order.Status.Status == OrderStatusSD.Cancelled || order.Status.Status == OrderStatusSD.Returned)
                {
                    _response.StatusCode = HttpStatusCode.Conflict;
                    _response.ErrorMessage.Add("It is not possible to change the order because it has already been canceled or returned!");
                    return _response;
                }
                
                var villaOccupiedSpecification =
                    new IsVillaOccupiedExceptOrderIdSpecification(order.VillaId,order.Id,updateDto.CheckIn,updateDto.CheckOut);

                var isVillaOccupied = await _unitOfWork.Villas.FindAny(villaOccupiedSpecification);

                if (isVillaOccupied)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("The villa is already booked for that time!");
                    return _response;
                }

                var orderStatusSpecification = new FindOrderStatusSpecification(OrderStatusSD.Updated);

                var orderStatus = await _unitOfWork.OrderStatus.FindSingle(orderStatusSpecification);

                if (orderStatus == null)
                {
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessage.Add($"Status {OrderStatusSD.Updated} was not found!");
                    return _response;
                }

                order.CheckIn = updateDto.CheckIn;
                order.CheckOut = updateDto.CheckOut;
                order.StatusId = orderStatus.Id;
                order.Status = orderStatus;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                var orderDto = _mapper.Map<OrderDto>(order);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = orderDto;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.FromHierarchy(e => e.InnerException).Select(e => e.Message).ToList();
            }
            return _response;
        }

        private static bool IsValidOrderDate(DateTime checkIn, DateTime checkOut)
        {
            return checkIn != DateTime.MinValue && checkOut != DateTime.MinValue && checkIn < checkOut && !DateTime.Equals(checkIn,checkOut) && checkOut > checkIn.AddDays(1);
        }
    }
}
