﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto.Order;
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
                    _response.StatusCode = HttpStatusCode.BadRequest;
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

        private static bool IsValidOrderDate(DateTime checkIn, DateTime checkOut)
        {
            return checkIn != DateTime.MinValue && checkOut != DateTime.MinValue && checkIn < checkOut && !DateTime.Equals(checkIn,checkOut) && checkOut > checkIn.AddDays(1);
        }
    }
}
