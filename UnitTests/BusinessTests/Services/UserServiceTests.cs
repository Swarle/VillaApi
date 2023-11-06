using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto.User;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Models;
using DataLayer.Repository.Interfaces;
using DataLayer.Specification.Infrastructure;
using DataLayer.Specification.UserSpecification;
using DataLayer.UnitOfWork.Interfaces;
using FluentAssertions;
using Moq;
using Utility;

namespace UnitTests.BusinessTests.Services
{
    public class UserServiceTests
    {
        private IMapper _mapper;
        private Mock<IRepository<Users>> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private IUserService _userService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = UnitTestHelper.GetMapper();
        }

        [SetUp]
        public void SetUp()
        {
            _userRepository = new Mock<IRepository<Users>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(e => e.Users).Returns(_userRepository.Object);
            _userService = new UserService(_unitOfWork.Object, _mapper);
        }

        #region GetAllUsersAsync

        [Test]
        public async Task GetAllUsersAsync_WhenUsersWereFound_ReturnSApiResponseWithStatusCode200()
        {
            //Arrange
            var source = new List<Users> 
            { 
                new Users
                {
                    Id = Guid.NewGuid(),
                    Login = "user1",
                    FirstName = "John",
                    LastName = "Doe",
                    HashedPassword = "hashed_password_1",
                    RoleId = Guid.NewGuid(),
                    Role = new Role
                    {
                        Id = Guid.NewGuid(),
                        RoleName = RolesSD.Customer
                    },
                    CreatedDate = DateTime.Now,
                }, 
                new Users
                {
                    Id = Guid.NewGuid(),
                    Login = "user2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    HashedPassword = "hashed_password_2",
                    RoleId = Guid.NewGuid(),
                    Role = new Role
                    {
                        Id = Guid.NewGuid(),
                        RoleName = RolesSD.Admin
                    },
                    CreatedDate = DateTime.Now,
                }

            };

            _userRepository.Setup(repo => repo.Find(It.IsAny<ISpecification<Users>>()))
                .ReturnsAsync(source);

            var expectedResult = _mapper.Map<List<UserPartialDto>>(source);

            //Act
            var response = await _userService.GetAllUsersAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccess.Should().BeTrue();
            response.ErrorMessage.Should().BeEmpty();
            response.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetAllUsersAsync_WhenUsersNotFound_ReturnApiResponseWithStatusCode404()
        {
            //Arrange
            _userRepository.Setup(repo => repo.Find(It.IsAny<FindUserWithRoleSpecification>()))
                .ReturnsAsync(new List<Users>());

            //Act
            var response = await _userService.GetAllUsersAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.IsSuccess.Should().BeTrue();
            response.Result.Should().BeNull();
            response.ErrorMessage.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetAllUsersAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            _userRepository.Setup(repo => repo.Find(It.IsAny<FindUserWithRoleSpecification>()))
                .ThrowsAsync(new Exception("Test exception"));

            //Act
            var response = await _userService.GetAllUsersAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.IsSuccess.Should().BeFalse();
            response.Result.Should().BeNull();
            response.ErrorMessage.Should().NotBeEmpty();
        }

        #endregion

        #region GetUserByIdAsync

        [Test]
        public async Task GetUserByIdAsync_WhenUserExist_ReturnsApiResponseWithStatusCode200()
        {
            //Arrange
            var id = Guid.NewGuid();

            var user = GetUser();

            _userRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Users>>())).ReturnsAsync(user);

            var expectedResult = _mapper.Map<UserDto>(user);

            //Act
            var action = await _userService.GetUserByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();
            
            var result = action.Result as UserDto;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetUserByIdAsync_WhenIdEmpty_ReturnsApiResponseWithStatusCode400()
        {
            //Arrange
            var id = Guid.Empty;

            //Act
            var action = await _userService.GetUserByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var id = Guid.NewGuid();

            _userRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Users>>())).ReturnsAsync(null as Users);

            //Act
            var action = await _userService.GetUserByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetUserByIdAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var id = Guid.NewGuid();

            _userRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Users>>()))
                .Throws(new Exception("Server error"));

            //Act
            var action = await _userService.GetUserByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }
        #endregion

        #region UpdateUserAsync

        [Test]
        public async Task UpdateUserAsync_WhenUpdated_ReturnsApiResponseWithStatusCode200()
        {
            //Arrange
            var updateDto = new UserUpdateDto
            {
                Id = Guid.NewGuid(),
                FirstName = "first name",
                LastName = "last name"
            };

            var user = GetUser();
            
            _userRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Users>>())).ReturnsAsync(user);

            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;

            var expectedResult = _mapper.Map<UserDto>(user);

            //Act
            var action = await _userService.UpdateUserAsync(updateDto);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as UserDto;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task UpdateUserAsync_WhenUserNotExist_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var updateDto = new UserUpdateDto
            {
                Id = Guid.NewGuid(),
                FirstName = "first name",
                LastName = "last name"
            };

            _userRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Users>>())).ReturnsAsync(null as Users);

            //Act
            var action = await _userService.UpdateUserAsync(updateDto);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task UpdateUserAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var updateDto = new UserUpdateDto
            {
                Id = Guid.NewGuid(),
                FirstName = "first name",
                LastName = "last name"
            };

            _userRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Users>>()))
                .Throws(new Exception("Server error"));

            //Act
            var action = await _userService.UpdateUserAsync(updateDto);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        #endregion

        private static Users GetUser()
        {
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var orderStatusId = Guid.NewGuid();
            var villaId = Guid.NewGuid();

            return new Users
            {
                Id = userId,
                CreatedDate = DateTime.Now,
                FirstName = "first name",
                LastName = "last name",
                HashedPassword = "password",
                Login = "login",
                Role = new Role
                {
                    Id = roleId,
                    RoleName = RolesSD.Customer
                },
                RoleId = roleId,
                Orders = new List<Orders>
                {
                    new Orders
                    {
                        Id = orderId,
                        CheckIn = DateTime.Now,
                        CheckOut = DateTime.Now.AddDays(7),
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        StatusId = orderStatusId,
                        UserId = userId,
                        Status = new OrderStatus
                        {
                            Id = orderStatusId,
                            Status = OrderStatusSD.Completed
                        },
                        Villa = new Villa
                        {
                            Id = villaId,
                            Describe = "describe",
                            ImageUrl = "image",
                            Name = "villa",
                            Price = 1
                        }
                    }
                }
            };
        }
    }
}
