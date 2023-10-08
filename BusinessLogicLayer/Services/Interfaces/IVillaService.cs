using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IVillaService
    {
        Task<ApiResponse> GetVillasPartialAsync();
        Task<ApiResponse> GetVillasAsync();
        Task<ApiResponse> GetVillaByIdAsync(Guid id);
        Task<ApiResponse> CreateVillaAsync(VillaCreateDto villaDto);
    }
}
