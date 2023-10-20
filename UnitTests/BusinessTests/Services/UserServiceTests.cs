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
    }
}
