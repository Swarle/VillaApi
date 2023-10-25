using AutoMapper;
using BusinessLogicLayer.Dto.Order;
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.Services;
using DataLayer.Models;
using DataLayer.UnitOfWork.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Repository.Interfaces;
using DataLayer.Specification.Infrastructure;
using FluentAssertions;
using Utility;

namespace UnitTests.BusinessTests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private IMapper _mapper;
        private Mock<IRepository<Orders>> _orderRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private IOrderService _orderService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = UnitTestHelper.GetMapper();
        }

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _orderRepository = new Mock<IRepository<Orders>>();
            _unitOfWork.Setup(e => e.Orders).Returns(_orderRepository.Object);
            _orderService = new OrderService(_unitOfWork.Object, _mapper);
        }

        [Test]
        public async Task GetOrderByIdAsync_WhenOrderExist_ReturnsApiResponseWithStatusCode200()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var statusId = Guid.NewGuid();
            var villaStatusId = Guid.NewGuid();
            var villaId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var order = new Orders
            {
                Id = orderId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                StatusId = statusId,
                UserId = userId,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                Status = new OrderStatus
                {
                    Id = statusId,
                    //TODO:Provide order statuses magic strings class
                    Status = StatusesSD.Available
                },
                User = new Users
                {
                    Id = userId,
                    Login = "Login",
                    FirstName = "Name",
                    LastName = "LastName",
                    HashedPassword = "Password",
                    RoleId = roleId,
                    CreatedDate = DateTime.Now,
                    Role = new Role
                    {
                        Id = roleId,
                        RoleName = RolesSD.Customer
                    }
                },
                Villa = new List<Villa>
                {
                    new Villa
                    {
                        Id = villaId,
                        Name = "Villa",
                        Describe = "Describe",
                        ImageUrl = "Image",
                        VillaNumber = 1,
                        StatusId = villaStatusId,
                        Price = 100.00M,
                        Status = new VillaStatus
                        {
                            Id = villaStatusId,
                            Status = StatusesSD.Available
                        }
                    },
                }
            };

            var orderDto = _mapper.Map<OrderDto>(order); 

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(order); 

            // Act
            var action = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();
            action.Result.Should().BeEquivalentTo(orderDto);
        }

        [Test]
        public async Task GetOrderByIdAsync_WhenEmptyId_ReturnsApiResponseWithStatusCode400()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act
            var action = await _orderService.GetOrderByIdAsync(emptyId);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetOrderByIdAsync_WhenOrderNotExist_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            var id = Guid.NewGuid(); 

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(null as Orders);

            // Act
            var action = await _orderService.GetOrderByIdAsync(id);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetOrderByIdAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            var exception = new Exception("Server error");
            var orderId = Guid.NewGuid();

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .Throws(exception); // Simulate an exception

            // Act
            var action = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            action.IsSuccess.Should().BeFalse();
            action.Result.Should().BeNull();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
        }
    }
}
