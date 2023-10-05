using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.UnitOfWork.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VillaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetVillasPartialAsync()
        {
            ApiResponse result = new ApiResponse();

            var villa = await _unitOfWork.Villas.GetAllAsync();

            if (!villa.Any())
            {
                result.StatusCode = HttpStatusCode.NotAcceptable;
                return result;
            }

            var villaPartialDto = _mapper.Map<IEnumerable<VillaPartialDto>>(villa);

            result.Result = villaPartialDto;
            result.StatusCode = HttpStatusCode.OK;

            return result;
        }
    }
}
