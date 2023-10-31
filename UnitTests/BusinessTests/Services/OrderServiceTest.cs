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
using DataLayer.Specification.VillaSpecification;
using FluentAssertions;
using Utility;

namespace UnitTests.BusinessTests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private IMapper _mapper;
        private Mock<IRepository<Orders>> _orderRepository;
        private Mock<IRepository<OrderStatus>> _orderStatusRepository;
        private Mock<IRepository<Users>> _userRepository;
        private Mock<IRepository<Villa>> _villaRepository;
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
            _orderStatusRepository = new Mock<IRepository<OrderStatus>>();
            _userRepository = new Mock<IRepository<Users>>();
            _villaRepository = new Mock<IRepository<Villa>>();
            _unitOfWork.Setup(e => e.Orders).Returns(_orderRepository.Object);
            _unitOfWork.Setup(e => e.OrderStatus).Returns(_orderStatusRepository.Object);
            _unitOfWork.Setup(e => e.Users).Returns(_userRepository.Object);
            _unitOfWork.Setup(e => e.Villas).Returns(_villaRepository.Object);
            _orderService = new OrderService(_unitOfWork.Object, _mapper);
        }

        #region GetOrderByIdAsync

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
                    Status = VillaStatusSD.Available
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
                Villa = new Villa
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
                            Status = VillaStatusSD.Available
                        }
                    },
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

        #endregion

        #region CreateOrderAsync

        [Test]
        public async Task CreateOrderAsync_WhenValidOrderData_ReturnsApiResponseWithStatusCode200()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7), 
                UserId = Guid.NewGuid(), 
                VillaId = Guid.NewGuid(),
            };

            var orderStatus = new OrderStatus
            {
                Id = Guid.NewGuid(),
                Status = OrderStatusSD.InProgress
            };
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var order = new Orders
            {
                Id = orderId,
                CheckIn = createDto.CheckIn,
                CheckOut = createDto.CheckOut,
                CreatedDate = DateTime.Now,
                Status = new OrderStatus
                {
                    Id = orderStatus.Id,
                    Status = orderStatus.Status,
                },
                StatusId = orderStatus.Id,
                UpdatedDate = DateTime.Now,
                User = new Users
                {
                    Id = userId,
                    CreatedDate = DateTime.Now,
                    FirstName = "Name",
                    HashedPassword = "Password",
                    LastName = "LastName",
                    Login = "Login",
                    RoleId = roleId,
                    Role = new Role
                    {
                        Id = roleId,
                        RoleName = RolesSD.Customer,
                    },
                },
                UserId = userId,
                Villa = new Villa
                {
                    Id = Guid.Parse("636C067C-3D13-4CB1-A7C6-8C570F6577C6"),
                    Describe = "Test data 1",
                    ImageUrl = "Image 1",
                    Name = "Name 1",
                    Price = 1,
                    StatusId = Guid.Parse("631CC936-E6EF-4D14-8642-553C750F2788"),
                    VillaDetails = new VillaDetails
                    {
                        Id = Guid.Parse("90C68D3B-7F05-4F7F-B3D3-AD27EEC89802"),
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Occupancy = 1,
                        Rate = 1,
                        Sqmt = 1,
                        VillaId = Guid.Parse("636C067C-3D13-4CB1-A7C6-8C570F6577C6"),
                    },
                    Status = new VillaStatus
                    {
                        Id = Guid.Parse("631CC936-E6EF-4D14-8642-553C750F2788"),
                        Status = VillaStatusSD.Available
                    }
                }
            };

            var expectedResult = _mapper.Map<OrderDto>(order);

            _userRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Users>>())).ReturnsAsync(true);

            _orderStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<OrderStatus>>()))
                .ReturnsAsync(orderStatus);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<FindVillaSpecification>()))
                .ReturnsAsync(true);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<IsVillaOccupied>()))
                .ReturnsAsync(false);
            
            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(order);

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();
            
            var result = action.Result as OrderDto;
            result.Should().BeEquivalentTo(expectedResult);

        }

        [Test]
        public async Task CreateOrderAsync_WhenInvalidOrderDate_ReturnsApiResponseWithStatusCode400()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                UserId = Guid.NewGuid(),
                VillaId = Guid.NewGuid(),
            };

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task CreateOrderAsync_WhenInvalidUserId_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
                UserId = Guid.NewGuid(),
                VillaId = Guid.NewGuid(),
            };

            _userRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Users>>()))
                .ReturnsAsync(false); 

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task CreateOrderAsync_WhenStatusNotFound_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
                UserId = Guid.NewGuid(),
                VillaId = Guid.NewGuid(),
            };

            _userRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Users>>())).ReturnsAsync(true);

            _orderStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<OrderStatus>>()))
                .ReturnsAsync(null as OrderStatus);

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task CreateOrderAsync_WhenInvalidVillaId_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
                UserId = Guid.NewGuid(),
                VillaId = Guid.NewGuid(), 
            };

            var orderStatus = new OrderStatus
            {
                Id = Guid.NewGuid(),
                Status = OrderStatusSD.InProgress
            };

            _userRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Users>>())).ReturnsAsync(true);

            _orderStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<OrderStatus>>()))
                .ReturnsAsync(orderStatus);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<FindVillaSpecification>()))
                .ReturnsAsync(false); 

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task CreateOrderAsync_WhenVillaOccupied_ReturnsApiResponseWithStatusCode400()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
                UserId = Guid.NewGuid(),
                VillaId = Guid.NewGuid(),
            };

            var orderStatus = new OrderStatus
            {
                Id = Guid.NewGuid(),
                Status = OrderStatusSD.InProgress
            };

            _userRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Users>>())).ReturnsAsync(true);

            _orderStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<OrderStatus>>()))
                .ReturnsAsync(orderStatus);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<FindVillaSpecification>()))
                .ReturnsAsync(true);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<IsVillaOccupied>()))
                .ReturnsAsync(true);

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task CreateOrderAsync_WhenExceptionThrown_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            var createDto = new OrderCreateDto
            {
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
                UserId = Guid.NewGuid(),
                VillaId = Guid.NewGuid(),
            };

            _userRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Users>>()))
                .Throws(new Exception("Server error"));

            // Act
            var action = await _orderService.CreateOrderAsync(createDto);

            // Assert
            action.IsSuccess.Should().BeFalse();
            action.Result.Should().BeNull();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
        }
        #endregion
    }
}
