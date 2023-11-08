using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.Villa;
using BusinessLogicLayer.Infastructure;
using BusinessLogicLayer.Infrastructure;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IVillaService
    {
        Task<ApiResponse> GetVillasPartialAsync(PaginationFilter filter);
        Task<ApiResponse> GetVillasAsync();
        Task<ApiResponse> GetVillaByIdAsync(Guid id);
        Task<ApiResponse> CreateVillaAsync(VillaCreateDto villaCreateDto);
        Task<ApiResponse> UpdateVillaAsync(VillaUpdateDto updateDto);
        Task<ApiResponse> DeleteVillaAsync(Guid id);
        Task<ApiResponse> GetVillaStatusesAsync();
    }
}
