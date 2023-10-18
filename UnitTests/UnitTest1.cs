//using System.Collections;
//using System.Net;
//using AutoMapper;
//using BusinessLogicLayer.Dto;
//using BusinessLogicLayer.Infastructure;
//using BusinessLogicLayer.Infrastructure;
//using Moq;
//using BusinessLogicLayer.Services;
//using BusinessLogicLayer.Services.Interfaces;
//using DataLayer.Models;
//using DataLayer.Repository;
//using DataLayer.Repository.Interfaces;
//using DataLayer.Specification.VillaSpecification;
//using DataLayer.UnitOfWork.Interfaces;

//namespace UnitTests
//{
//    public class Tests
//    {
//        [SetUp]
//        public void Setup()
//        {

//        }

//        [Test]
//        public async Task Test1()
//        {
//            //Arrange
//            var mockVillaRepository = new Mock<IRepository<Villa>>();

//            var listVilla = new List<Villa>
//            {
//                new Villa
//                {
//                    Id = Guid.NewGuid(),
//                    Describe = "Test data",
//                    ImageUrl = "image",
//                    Price = 2000,
//                    StatusId = Guid.NewGuid(),
//                    VillaDetailsId = Guid.NewGuid()
//                },
//                new Villa
//                {
//                    Id = Guid.NewGuid(),
//                    Describe = "Test data",
//                    ImageUrl = "image",
//                    Price = 2000,
//                    StatusId = Guid.NewGuid(),
//                    VillaDetailsId = Guid.NewGuid()
//                }
//            };

//            mockVillaRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(listVilla);
            
//            var mockUnitOfWork = new Mock<IUnitOfWork>();
//            mockUnitOfWork.Setup(u => u.Villas).Returns(mockVillaRepository.Object);

//            var mapProfile = new MappingConfig();
//            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mapProfile));
//            IMapper mapper = new Mapper(configuration);

//            var villaService = new VillaService(mockUnitOfWork.Object, mapper);
            
//            //Act
//            var result = await villaService.GetVillasPartialAsync();

//            //Assert

//            var resultList = (result.Result as IEnumerable<VillaPartialDto>).ToList();
//            var mappedListVilla = mapper.Map<List<VillaPartialDto>>(listVilla);



//            Assert.Multiple(() =>
//            {
//                Assert.AreEqual((int)result.StatusCode,(int)HttpStatusCode.OK);

//                Assert.AreEqual(true, result.IsSuccess);
                
//                Assert.IsNotNull(resultList);
//            });


//        }
//    }
//}