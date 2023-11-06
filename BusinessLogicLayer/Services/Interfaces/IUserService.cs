using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Infrastructure;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse> GetAllUsersAsync();
        Task<ApiResponse> GetUserByIdAsync(Guid id);
    }
}
