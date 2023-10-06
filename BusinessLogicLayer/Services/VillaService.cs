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
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;
using DataLayer.Specification.VillaSpecification;
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

            var villas = await _unitOfWork.Villas.GetAllAsync();

            if (!villas.Any())
            {
                result.StatusCode = HttpStatusCode.NotAcceptable;
                return result;
            }

            var villaPartialDto = _mapper.Map<IEnumerable<VillaPartialDto>>(villas);

            result.Result = villaPartialDto;
            result.StatusCode = HttpStatusCode.OK;

            return result;
        }

        public async Task<ApiResponse> GetVillasAsync()
        {
            ApiResponse result = new ApiResponse();

            ISpecification<Villa> specification = new VillaWithDetailsAndStatusSpecification();

            var villas = await _unitOfWork.Villas.Find(specification);

            if (villas.Any(x => specification.IsSatisfied(x)))
            {
                //TODO: Make new exception for these situations
                throw new Exception("The received entities do not match the predicate");
            }

            var villasDto = _mapper.Map<IEnumerable<VillaDto>>(villas);

            result.Result = villasDto;
            result.StatusCode = HttpStatusCode.OK;
            return result;
        }
    }
}
