using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterUserAsync(RegistrationDto registrationDto);
        Task<ApiResponse> LoginAsync(LoginDto loginDto);
    }
}
