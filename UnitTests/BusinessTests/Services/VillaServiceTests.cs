using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Models;
using DataLayer.Repository;
using DataLayer.Repository.Interfaces;
using DataLayer.Specification.Infrastructure;
using DataLayer.Specification.VillaSpecification;
using DataLayer.UnitOfWork;
using DataLayer.UnitOfWork.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Moq;
using UnitTests.BusinessTests.EqualityComparers;
using Utility;
using VillaDto = BusinessLogicLayer.Dto.VillaDto;

namespace UnitTests.BusinessTests.Services
{
    [TestFixture]
    public class VillaServiceTests
    {
        private IMapper _mapper;
        private Mock<IRepository<Villa>> _villaRepository;
        private Mock<IRepository<VillaStatus>> _villaStatusRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private IVillaService _villaService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = UnitTestHelper.GetMapper();
        }

        [SetUp]
        public void SetUp()
        {
            _villaRepository = new Mock<IRepository<Villa>>();
            _villaStatusRepository = new Mock<IRepository<VillaStatus>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(e => e.Villas).Returns(_villaRepository.Object);
            _unitOfWork.Setup(e => e.VillaStatus).Returns(_villaStatusRepository.Object);
            _villaService = new VillaService(_unitOfWork.Object, _mapper);
        }

        #region GetVillasPartialAsync
        [Test]
        public async Task GetVillasPartialAsync_WhenTryingGetAll_ReturnsApiResponseWithStatusCode200AndVillasPartialDtoList()
        {
            //Arrange
            var source = new List<Villa>
            {
                new Villa
                {
                    Id = Guid.NewGuid(),
                    Describe = "Test data",
                    ImageUrl = "image",
                    Price = 2000,
                    StatusId = Guid.NewGuid(),
                },
                new Villa
                {
                    Id = Guid.NewGuid(),
                    Describe = "Test data 2",
                    ImageUrl = "image 2",
                    Price = 2001,
                    StatusId = Guid.NewGuid(),
                }
            };
            
            _villaRepository.Setup(e => e.GetAllAsync()).ReturnsAsync(source);

            var expectedResult = _mapper.Map<List<VillaPartialDto>>(source);

            //Act
            var action = await _villaService.GetVillasPartialAsync();

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();
            action.Result.Should().NotBeNull();

            var result = action.Result as List<VillaPartialDto>;
            result.Should().BeEquivalentTo(expectedResult, o => o.Using(new VillaPartialDtoEqualityComparer()));

        }

        [Test]
        public async Task GetVillasPartialAsync_WhenVillasNotFound_ReturnsApiResponseWithStatusCodeNotFoundAndErrorMessage()
        {
            //Arrange
            _villaRepository.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<Villa>());

            //Act
            var action = await _villaService.GetVillasPartialAsync();

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();


        }

        [Test]
        public async Task GetVillasPartialAsync_WhenThrowingException_ReturnsApiResponseWithIsSuccessIsFalseAndStatusCode500()
        {
            //Arrange
            var exception = new Exception("Server error");
            var expectedError = exception.ToString();

            _villaRepository.Setup(e => e.GetAllAsync()).Throws(exception);

            //Act
            var action = await _villaService.GetVillasPartialAsync();

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().Contain(e => e.Contains(expectedError));
            action.Result.Should().BeNull();
        }
        #endregion

        #region GetVillasAsync

        private List<Villa> GetVillaFullList()
        {
            return new List<Villa>
            {
                new Villa
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
                        Status = StatusesSD.Available
                    }
                },
                new Villa
                {
                    Id = Guid.Parse("D87EFFEB-FE6A-4EC8-9954-D1DEEBC29D7E"),
                    Describe = "Test data 2",
                    ImageUrl = "Image 2",
                    Name = "Name 2",
                    Price = 2,
                    StatusId = Guid.Parse("1B8C6DF0-DD9C-4CB6-AC63-E10B345CB077"),
                    VillaDetails = new VillaDetails
                    {
                        Id = Guid.Parse("21AB23FF-BB9D-4BEB-BE6D-A5969A6D190C"),
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Occupancy = 2,
                        Rate = 2,
                        Sqmt = 2,
                        VillaId = Guid.Parse("D87EFFEB-FE6A-4EC8-9954-D1DEEBC29D7E")
                    },
                    Status = new VillaStatus
                    {
                        Id = Guid.Parse("1B8C6DF0-DD9C-4CB6-AC63-E10B345CB077"),
                        Status = StatusesSD.Available,
                    }
                }
            };
        }

        [Test]
        public async Task GetVillasAsync_WhenVillasWereFound_ReturnApiResponseWithStatusCode200AndVillaDtoList()
        {
            //Arrange
            var source = GetVillaFullList();
            
            _villaRepository.Setup(e => e.Find(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(source);

            var expectedList = _mapper.Map<List<VillaDto>>(source);
            
            //Act
            var action = await _villaService.GetVillasAsync();

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().BeEmpty();
            action.Result.Should().NotBeNull();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var resultList = action.Result as List<VillaDto>;
            resultList.Should().BeEquivalentTo(expectedList, o => o.Using(new VillaDtoEqualityComparer()));

        }

        [Test]
        public async Task GetVillasAsync_WhenVillasWereNotFound_ReturnApiResponseWithStatusCode404AndErrorList()
        {
            //Arrange
            var source = new List<Villa>();

            _villaRepository.Setup(e => e.Find(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(source);
            
            //Act
            var action = await _villaService.GetVillasAsync();

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetVillasAsync_WhenThrowingException_ReturnApiResponseWithIsSuccessFalseAndInternalSeverErrorStatusCode()
        {
            //Arrange
            var exception = new Exception("Server error");
            _villaRepository.Setup(e => e.Find(It.IsAny<ISpecification<Villa>>()))
                .ThrowsAsync(exception);
            
            //Act
            var action = await _villaService.GetVillasAsync();

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        #endregion

        #region GetVillaByIdAsync

        [Test]
        public async Task GetVillaByIdAsync_WhenVillaWereFoundAndIdIsCorrect_ReturnsApiResponseWithStatusCode200AndVillaDtoAsResult()
        {
            //Arrange
            var source = GetVillaFullList().First();

            var id = source.Id;

            var expectedResult = _mapper.Map<VillaDto>(source);

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(source);

            //Act
            var action = await _villaService.GetVillaByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().BeEmpty();
            action.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = action.Result as VillaDto;
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetVillaByIdAsync_WhenIdIsEmpty_ReturnsApiResponseWithStatusCode400()
        {
            //Arrange
            var id = Guid.Empty;

            //Act
            var action = await _villaService.GetVillaByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().NotBeEmpty();
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task GetVillaByIdAsync_WhenVillaNotExist_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var id = Guid.NewGuid();
            
            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync((Villa)null);

            //Act
            var action = await _villaService.GetVillaByIdAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetVillaByIdAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var exception = new Exception("Server error");
            
            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).Throws(exception);

            //Act
            var action = await _villaService.GetVillaByIdAsync(Guid.NewGuid());

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }
        #endregion

        #region CreateVillaAsync

        [Test]
        public async Task CreateVillaAsync_WhenWasCreated_ReturnsApiResponseWithStatusCode200()
        {
            //Arrange
            var villaCreateDto = new VillaCreateDto
            {
                Name = "Villa",
                Describe = "Describe",
                ImageUrl = "Image",
                Occupancy = 1,
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                VillaNumber = 1
            };

            var villaStatus = new VillaStatus
            {
                Id = Guid.NewGuid(),
                Status = StatusesSD.Available
            };

            var expectedResult = _mapper.Map<VillaDto>(_mapper.Map<Villa>(villaCreateDto));

            expectedResult.VillaStatus = villaStatus.Status;

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>()))
                .ReturnsAsync(null as Villa);
            _villaStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<VillaStatus>>())).ReturnsAsync(villaStatus);
            

            //Act
            var action =  await _villaService.CreateVillaAsync(villaCreateDto);

            //Assert
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.IsSuccess.Should().BeTrue();
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as VillaDto;
            result.Should().BeEquivalentTo(expectedResult, o =>
            {
                o.Excluding(v => v.Id);
                return o;
            });

        }

        //[Test]
        //public async Task CreateVillaAsync_WhenVillaCreateDtoEmpty_ReturnApiResponseWithStatusCode400()
        //{
        //    //Arrange
        //    var villaCreateDto = new VillaCreateDto
        //    {
        //        Name = "Villa"
        //    };

        //    var expectedErrorList = new List<string> { "Villa is null or empty!" };

        //    //Act
        //    var action = await _villaService.CreateVillaAsync(villaCreateDto);

        //    //Assert
        //    action.IsSuccess.Should().BeTrue();
        //    action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //    action.ErrorMessage.Should().BeEquivalentTo(expectedErrorList);
        //    action.Result.Should().BeNull();
        //}

        [Test]
        public async Task CreateVillaAsync_WhenVillaAlreadyExist_ReturnsApiResponseWithStatusCode400()
        {
            //Arrange
            var villaCreateDto = new VillaCreateDto
            {
                Name = "Villa",
                Describe = "Describe",
                ImageUrl = "Image",
                Occupancy = 1,
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                VillaNumber = 1
            };

            var villa = _mapper.Map<Villa>(villaCreateDto);
            
            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(villa);

            //Act
            var action = await _villaService.CreateVillaAsync(villaCreateDto);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }

        [Test]
        public async Task CreateVillaAsync_WhenVillaStatusNotExist_ReturnsApiResponseWithStatusCode500()
        {
            var villaCreateDto = new VillaCreateDto
            {
                Name = "Villa",
                Describe = "Describe",
                ImageUrl = "Image",
                Occupancy = 1,
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                VillaNumber = 1
            };
            
            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(null as Villa);
            _villaStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<VillaStatus>>()))
                .ReturnsAsync(null as VillaStatus);

            //Act
            var action = await _villaService.CreateVillaAsync(villaCreateDto);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }

        [Test]
        public async Task CreateVillaAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            var villaCreateDto = new VillaCreateDto
            {
                Name = "Villa",
                Describe = "Describe",
                ImageUrl = "Image",
                Occupancy = 1,
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                VillaNumber = 1
            };

            var exception = new Exception("Server error");


            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).Throws(exception);

            //Act
            var action = await _villaService.CreateVillaAsync(villaCreateDto);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.ErrorMessage.Should().NotBeEmpty();
            action.Result.Should().BeNull();
        }
        #endregion

        #region UpdateVillaAsync

        [Test]
        public async Task UpdateVillaAsync_WhenWasUpdated_ReturnsApiResponseWithStatusCode200()
        {
            //Arrange
            var updateDto = new VillaUpdateDto
            {
                Id = Guid.NewGuid(),
                Describe = "Test",
                ImageUrl = "Image",
                Occupancy = 1,
                Name = "Villa",
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                Status = StatusesSD.Available,
                VillaNumber = 1
            };

            var villaStatus = new VillaStatus
            {
                Id = Guid.NewGuid(),
                Status = StatusesSD.Available
            };

            var villa = _mapper.Map<Villa>(updateDto);
            villa.Status = villaStatus;
            villa.StatusId = villaStatus.Id;

            var expectedVilla = _mapper.Map<VillaDto>(villa);

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>()))
                .ReturnsAsync(villa);
            _villaStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<VillaStatus>>()))
                .ReturnsAsync(villaStatus);

            //Act
            var action = await _villaService.UpdateVillaAsync(updateDto);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as VillaDto;
            result.Should().BeEquivalentTo(expectedVilla);
        }

        [Test]
        public async Task UpdateVillaAsync_WhenVillaNotExist_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var updateDto = new VillaUpdateDto
            {
                Id = Guid.NewGuid(),
                Describe = "Test",
                ImageUrl = "Image",
                Occupancy = 1,
                Name = "Villa",
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                Status = StatusesSD.Available,
                VillaNumber = 1
            };
            
            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(null as Villa);

            //Act
            var action = await _villaService.UpdateVillaAsync(updateDto);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }

        [Test]
        public async Task UpdateVillaAsync_WhenVillaStatusNotExist_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var updateDto = new VillaUpdateDto
            {
                Id = Guid.NewGuid(),
                Describe = "Test",
                ImageUrl = "Image",
                Occupancy = 1,
                Name = "Villa",
                Price = 1,
                Rate = 1,
                Sqmt = 1,
                Status = StatusesSD.Available,
                VillaNumber = 1
            };

            var villa = _mapper.Map<Villa>(updateDto);

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(villa);
            _villaStatusRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<VillaStatus>>())).ReturnsAsync(null as VillaStatus);

            //Act
            var action = await _villaService.UpdateVillaAsync(updateDto);

            //Assert
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.IsSuccess.Should().BeTrue();
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();

        }

        [Test]
        public async Task UpdateVillaAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var exception = new Exception("Server error");

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).Throws(exception);

            //Act
            var action = await _villaService.UpdateVillaAsync(It.IsAny<VillaUpdateDto>());

            //Assert
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.IsSuccess.Should().BeFalse();
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }
        #endregion

        #region DeleteVillaAsync

        [Test]
        public async Task DeleteVillaAsync_WhenVillaWasDeleted_ReturnsApiResponseWithStatusCode200()
        {
            //Arrange
            var id = Guid.NewGuid();
            var statusId = Guid.NewGuid();
            var villaDetailsId = Guid.NewGuid();

            var villa = new Villa
            {
                Id = id,
                Describe = "Test",
                ImageUrl = "Image",
                Name = "Villa",
                Price = 1,
                Status = new VillaStatus
                {
                    Id = statusId,
                    Status = StatusesSD.Available
                },
                StatusId = statusId,
                VillaDetails = new VillaDetails
                {
                    Id = villaDetailsId,
                    CreatedDate = DateTime.Now,
                    Occupancy = 1,
                    Rate = 1,
                    Sqmt = 1,
                    UpdatedDate = DateTime.Now,
                    VillaId = id,
                },
            };

            villa.VillaDetails.Villa = villa;

            var expectedResult = _mapper.Map<VillaDto>(villa);

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(villa);

            var villaDetailsRepository = new Mock<IRepository<VillaDetails>>();
            _unitOfWork.Setup(e => e.VillaDetails).Returns(villaDetailsRepository.Object);

            //Act
            var action = await _villaService.DeleteVillaAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.OK);
            action.ErrorMessage.Should().BeEmpty();

            var result = action.Result as VillaDto;
            result.Should().BeEquivalentTo(expectedResult, c => c.Using(new VillaDtoEqualityComparer()));
        }

        [Test]
        public async Task DeleteVillaAsync_WhenIdIsEmpty_ReturnsApiResponseWithStatusCode400()
        {
            //Arrange
            var id = Guid.Empty;

            //Act
            var action = await _villaService.DeleteVillaAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();
        }

        [Test]
        public async Task DeleteVillaAsync_WhenVillaNotExist_ReturnsApiResponseWithStatusCode404()
        {
            //Arrange
            var id = Guid.NewGuid();

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).ReturnsAsync(null as Villa);

            //Act
            var action = await _villaService.DeleteVillaAsync(id);

            //Assert
            action.IsSuccess.Should().BeTrue();
            action.StatusCode.Should().Be(HttpStatusCode.NotFound);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();

        }

        [Test]
        public async Task DeleteVillaAsync_WhenThrowingException_ReturnsApiResponseWithStatusCode500()
        {
            //Arrange
            var id = Guid.NewGuid();

            var exception = new Exception("Server error");

            _villaRepository.Setup(e => e.FindSingle(It.IsAny<ISpecification<Villa>>())).Throws(exception);

            //Act
            var action = await _villaService.DeleteVillaAsync(id);

            //Assert
            action.IsSuccess.Should().BeFalse();
            action.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            action.Result.Should().BeNull();
            action.ErrorMessage.Should().NotBeEmpty();

        }
        #endregion
    }
}
