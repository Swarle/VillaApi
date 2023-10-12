using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Models;
using DataLayer.Specification.RoleSpecification;
using DataLayer.Specification.UserSpecification;
using DataLayer.UnitOfWork.Interfaces;


namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JwtTokenHandler _tokenHandler;
        private ApiResponse _response;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, JwtTokenHandler tokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
            _tokenHandler = tokenHandler;
        }

        public async Task<ApiResponse> RegisterUserAsync(RegistrationDto registrationDto)
        {
            try
            {
                var userSpecification = new FindUserByLoginSpecification(registrationDto.Login);

                var isExistUser = await _unitOfWork.Users.FindSingle(userSpecification);

                if (isExistUser != null)
                {
                    _response.ErrorMessage.Add("User with this login already exist");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                var user = _mapper.Map<Users>(registrationDto);

                var roleSpecification = new FindRoleSpecification(registrationDto.Role);

                var role = await _unitOfWork.Role.FindSingle(roleSpecification);

                if (role == null)
                {
                    _response.ErrorMessage.Add("Role does not exist");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                user.Role = role;
                user.RoleId = role.Id;
                user.HashedPassword = PasswordHasher.HashPassword(registrationDto.Password);

                await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);

                var token = _tokenHandler.CreateJwtToken(userDto.Id, userDto.Login, userDto.Role);

                var authDto = new AuthResponseDto
                {
                    Token = token,
                    User = userDto,
                };

                _response.Result = authDto;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage.Add(ex.Message);
            }

            return _response;
        }

        public async Task<ApiResponse> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var userUniqueSpecification = new FindUserByLoginSpecification(loginDto.Login,includeRole:true);

                var user = await _unitOfWork.Users.FindSingle(userUniqueSpecification);

                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"User with login \"{loginDto.Login}\" does not exist");
                    return _response;
                }

                var isPasswordVerified = PasswordHasher.VerifyHashedPassword(user.HashedPassword, loginDto.Password);

                if (!isPasswordVerified)
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.ErrorMessage.Add("Password does not match");
                    return _response;
                }

                var token = _tokenHandler.CreateJwtToken(user.Id, user.Login, user.Role.RoleName);

                var userDto = _mapper.Map<UserDto>(user);

                var authDto = new AuthResponseDto()
                {
                    Token = token,
                    User = userDto
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = authDto;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
