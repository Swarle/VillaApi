using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto.User;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Specification.UserSpecification;
using DataLayer.UnitOfWork.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetAllUsersAsync()
        {
            try
            {
                var specification = new FindUserWithRoleSpecification();

                var users = await _unitOfWork.Users.Find(specification);

                if (!users.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("No users were founded!");
                    return _response;
                }

                var usersDto = _mapper.Map<List<UserPartialDto>>(users);

                _response.Result = usersDto;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = new List<string> { e.ToString() };
            }
            return _response;
        }

        //TODO: End this method when the OrderService is done
        //public async Task<ApiResponse> GetUserByIdAsync(Guid id)
        //{
        //    try
        //    {
        //        if (id == Guid.Empty)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.ErrorMessage.Add("Id is empty!");
        //            return _response;
        //        }

        //        var specification = new FindUserWithRoleSpecification(id);

        //        var user = await _unitOfWork.Users.FindSingle(specification);

        //        if (user == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            _response.ErrorMessage.Add("User with this id not exist!");
        //            return _response;
        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.ErrorMessage = new List<string> { e.ToString() };
        //    }

        //    return _response;
        //}
    }
}
