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
using Microsoft.AspNetCore.Http.HttpResults;

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

        #region GetOrdersAsync

        [Test]
        public async Task GetOrdersAsync_WhenOrdersExist_ReturnsApiResponseWithStatusCode200()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var statusId = Guid.NewGuid();
            var villaId = Guid.NewGuid();
            var villaStatusId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            var orders = new List<Orders>
            {
                new Orders
                {
                    Id = orderId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    StatusId = statusId,
                    UserId = userId,
                    VillaId = villaId,
                    CheckIn = DateTime.Now,
                    CheckOut = DateTime.Now.AddDays(7),
                    Status = new OrderStatus
                    {
                        Id = statusId,
                        Status = OrderStatusSD.Completed
                    },
                    User = new Users
                    {
                        Id = userId,
                        Login = "TestUser",
                        FirstName = "John",
                        LastName = "Doe",
                        HashedPassword = "hashed_password",
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
                        Name = "VillaName",
                        Describe = "VillaDescription",
                        ImageUrl = "Image.jpg",
                        VillaNumber = 1,
                        StatusId = villaStatusId,
                        Price = 100.00M,
                        Status = new VillaStatus
                        {
                            Id = villaStatusId,
                            Status = VillaStatusSD.Available,
                        }
                    }
                }
            };

            var expectedResult = _mapper.Map<List<OrderPartialDto>>(orders);

            _orderRepository.Setup(e => e.Find(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(orders);

            // Act
            var action = await _orderService.GetOrdersAsync();

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();
            
            var result = action.Result as List<OrderPartialDto>;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetOrdersAsync_WhenNoOrdersExist_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            _orderRepository.Setup(e => e.Find(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(new List<Orders>());

            // Act
            var action = await _orderService.GetOrdersAsync();

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetOrdersAsync_WhenExceptionThrown_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            _orderRepository.Setup(e => e.Find(It.IsAny<ISpecification<Orders>>()))
                .Throws(new Exception("Server error"));

            // Act
            var action = await _orderService.GetOrdersAsync();

            // Assert
            action.IsSuccess.Should().BeFalse();
            action.Result.Should().BeNull();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
        }

        #endregion

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
                .Throws(exception);

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
        public async Task CreateOrderAsync_WhenVillaOccupied_ReturnsApiResponseWithStatusCode409()
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
            action.StatusCode.Should().Be(HttpStatusCode.Conflict);
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

        #region UpdateOrderDto

        [Test]
        public async Task UpdateOrderAsync_WhenValidRequest_ReturnsApiResponseWithStatusCode200()
        {
            // Arrange

            var order = GetOrder();

            var updateDto = new OrderUpdateDto
            {
                Id = order.Id,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
            };

            var orderStatus = new OrderStatus
            {
                Id = Guid.NewGuid(),
                Status = OrderStatusSD.InProgress
            };

            var expectedResult = _mapper.Map<OrderDto>(order);

            expectedResult.CheckIn = updateDto.CheckIn;
            expectedResult.CheckOut = updateDto.CheckOut;
            expectedResult.Status = orderStatus.Status;
            
            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(order);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Villa>>()))
                .ReturnsAsync(false);

            _orderStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<OrderStatus>>()))
                .ReturnsAsync(orderStatus);

            // Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as OrderDto;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task UpdateOrderAsync_WhenDataInvalid_ReturnsApiResponseWithStatusCode400()
        {
            //Arrange
            var updateDto = new OrderUpdateDto
            {
                Id = Guid.NewGuid(),
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
            };

            //Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            //Assert
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task UpdateOrderAsync_WhenOrderNotFound_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            var updateDto = new OrderUpdateDto
            {
                Id = Guid.NewGuid(),
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
            };

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync((Orders)null);

            // Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task UpdateOrderAsync_WhenOrderAlreadyCancelled_ReturnsApiResponseWithStatusCode409()
        {
            // Arrange
            var order = GetOrder();

            order.Status.Status = OrderStatusSD.Cancelled;

            var updateDto = new OrderUpdateDto
            {
                Id = order.Id,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
            };
            
            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(order);

            // Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.Conflict);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task UpdateOrderAsync_WhenVillaOccupied_ReturnsApiResponseWithStatusCode400()
        {
            // Arrange
            var order = GetOrder();

            var updateDto = new OrderUpdateDto
            {
                Id = order.Id,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
            };


            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(order);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Villa>>()))
                .ReturnsAsync(true);

            // Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task UpdateOrderAsync_WhenOrderStatusNotFound_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            var order = GetOrder();

            var updateDto = new OrderUpdateDto
            {
                Id = order.Id,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
            };
            
            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(order);

            _villaRepository.Setup(e => e.FindAny(It.IsAny<ISpecification<Villa>>()))
                .ReturnsAsync(false);

            _orderStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<OrderStatus>>()))
                .ReturnsAsync((OrderStatus)null); 

            // Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task UpdateOrderAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var updateDto = new OrderUpdateDto
            {
                Id = Guid.NewGuid(),
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(7),
            };

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>()))
                .Throws(new Exception("Server error"));

            //Act
            var action = await _orderService.UpdateOrderAsync(updateDto);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }

        private Orders GetOrder()
        {

            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var orderStatusId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            return new Orders
            {
                Id = orderId,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                CreatedDate = DateTime.Now,
                Status = new OrderStatus
                {
                    Id = orderStatusId,
                    Status = OrderStatusSD.InProgress,
                },
                StatusId = orderStatusId,
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
        }
        #endregion

        #region GetOrderStatusesAsync

        [Test]
        public async Task GetOrderStatusesAsync_StatusesExist_ReturnsApiResponseWithStatusCode200()
        {
            // Arrange
            var orderStatuses = new List<OrderStatus>
            {
                new OrderStatus { Id = Guid.NewGuid(), Status = "Status1" },
                new OrderStatus { Id = Guid.NewGuid(), Status = "Status2" }

            };

            var expectedResult = _mapper.Map<List<OrderStatusDto>>(orderStatuses);

            _orderStatusRepository.Setup(u => u.GetAllAsync()).ReturnsAsync(orderStatuses);

            // Act
            var action = await _orderService.GetOrderStatusesAsync();

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as List<OrderStatusDto>;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetOrderStatusesAsync_WhenNoStatusesFound_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            var orderStatuses = new List<OrderStatus>();

            _orderStatusRepository.Setup(u => u.GetAllAsync()).ReturnsAsync(orderStatuses);

            // Act
            var action = await _orderService.GetOrderStatusesAsync();

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetOrderStatusesAsync_ThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            _orderStatusRepository.Setup(u => u.GetAllAsync()).Throws(new Exception("Server error"));

            // Act
            var action = await _orderService.GetOrderStatusesAsync();

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.IsSuccess.Should().BeFalse();
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        #endregion

        #region GetOrdersByUserIdAsync

        [Test]
        public async Task GetOrdersByUserIdAsync_WhenOrdersExist_ReturnsApiResponseWithStatusCode200()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var orders = new List<Orders> { GetOrder(),GetOrder() };

            var ordersDto = _mapper.Map<List<OrderDto>>(orders);

            _orderRepository.Setup(e => e.Find(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(orders);

            // Act
            var action = await _orderService.GetOrdersByUserIdAsync(userId);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();
            action.Result.Should().BeEquivalentTo(ordersDto);
        }

        [Test]
        public async Task GetOrdersByUserIdAsync_WhenEmptyId_ReturnsApiResponseWithStatusCode400()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var action = await _orderService.GetOrdersByUserIdAsync(userId);

            // Assert
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetOrdersByUserIdAsync_WhenOrdersNotExist_ReturnsApiResponseWithStatusCode404()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var orders = new List<Orders>();

            _orderRepository.Setup(e => e.Find(It.IsAny<ISpecification<Orders>>()))
                .ReturnsAsync(orders);

            // Act
            var action = await _orderService.GetOrderByIdAsync(userId);

            // Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetOrdersByUserIdAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            // Arrange
            var exception = new Exception("Server error");
            var orderId = Guid.NewGuid();

            _orderRepository.Setup(e => e.Find(It.IsAny<ISpecification<Orders>>()))
                .Throws(exception); 

            // Act
            var action = await _orderService.GetOrdersByUserIdAsync(orderId);

            // Assert
            action.IsSuccess.Should().BeFalse();
            action.Result.Should().BeNull();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
        }

        #endregion
        #region DeleteOrderAsync

        [Test]
        public async Task DeleteOrderAsync_WhenDeleted_ReturnsApiResponseWithStatusCode200()
        {
            //Arrange
            var orderId = Guid.NewGuid();

            var order = GetOrder();

            var expectedResult = _mapper.Map<OrderDto>(order);

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>())).ReturnsAsync(order);

            //Act
            var action = await _orderService.DeleteOrderAsync(orderId);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as OrderDto;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task DeleteOrderAsync_WhenInvalidId_ReturnsApiResponseWithStatusCode400()
        {
            //Arrange
            var orderId = Guid.Empty;

            //Act
            var action = await _orderService.DeleteOrderAsync(orderId);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task DeleteOrderAsync_WhenOrderNotFound_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var orderId = Guid.NewGuid();

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>())).ReturnsAsync(null as Orders);

            //Act
            var action = await _orderService.DeleteOrderAsync(orderId);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task DeleteOrderAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var orderId = Guid.NewGuid();

            _orderRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Orders>>())).Throws(new Exception("Server error"));

            //Act
            var action = await _orderService.DeleteOrderAsync(orderId);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        #endregion
    }
}
