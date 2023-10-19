using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services.Interfaces;
using Utility;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;
using DataLayer.Specification.VillaSpecification;
using DataLayer.Specification.VillaStatusSpecification;
using DataLayer.UnitOfWork.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Azure;

namespace BusinessLogicLayer.Services
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApiResponse _response;

        public VillaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetVillasPartialAsync()
        {
            try
            {
                var villas = await _unitOfWork.Villas.GetAllAsync();

                if (!villas.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("No Villas were found!");
                    return _response;
                }

                var villaPartialDto = _mapper.Map<IEnumerable<VillaPartialDto>>(villas);

                _response.Result = villaPartialDto;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }

            return _response;
        }

        public async Task<ApiResponse> GetVillasAsync()
        {
            try
            {
                ISpecification<Villa> specification = new VillaWithDetailsAndStatusSpecification();

                var villas = await _unitOfWork.Villas.Find(specification);

                if (!villas.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("No Villas were found!");
                    return _response;
                }

                var villasDto = _mapper.Map<IEnumerable<VillaDto>>(villas);

                _response.Result = villasDto;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        public async Task<ApiResponse> GetVillaByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty )
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("Id is Guid Empty!");
                    return _response;
                }

                var specification = new VillaWithDetailsAndStatusSpecification(id);
                var villa = await _unitOfWork.Villas.FindSingle(specification);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("Villa with this id not exist!");
                    return _response;
                }

                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = new List<string>{ex.ToString()};
            }

            return _response;
        }

        public async Task<ApiResponse> CreateVillaAsync(VillaCreateDto villaCreateDto)
        {
            try
            {
                var villaSpecification = new FindVillaSpecification(villaCreateDto.Name, villaCreateDto.VillaNumber);

                var isExist = await _unitOfWork.Villas.FindSingle(villaSpecification);

                if (isExist != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("Villa already exists!");
                    return _response;
                }

                var villa = _mapper.Map<Villa>(villaCreateDto);

                var villaStatusSpecification = new FindVillaStatusSpecification(StatusesSD.Available);

                var status = await _unitOfWork.VillaStatus.FindSingle(villaStatusSpecification);

                if (status == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessage.Add($"Status {StatusesSD.Available} does not exist!");
                    return _response;
                }

                villa.Status = status;
                villa.StatusId = status.Id;


                await _unitOfWork.Villas.CreateAsync(villa);
                await _unitOfWork.SaveChangesAsync();


                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            } 

            return _response;
        }

        public async Task<ApiResponse> UpdateVillaAsync(VillaUpdateDto updateDto)
        {
            try
            {
                var villaSpecification = new VillaWithDetailsAndStatusSpecification(updateDto.Id, true);

                var villaFromDb = await _unitOfWork.Villas.FindSingle(villaSpecification);

                if (villaFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("Villa does not exist!");
                    return _response;
                }

                var villaStatusSpecification = new FindVillaStatusSpecification(updateDto.Status);

                var status = await _unitOfWork.VillaStatus.FindSingle(villaStatusSpecification);

                if (status == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Status {updateDto.Status} does not exist!");
                    return _response;
                }

                var villa = _mapper.Map<Villa>(updateDto);

                villa.StatusId = status.Id;
                villa.Status = status;
                villa.VillaDetails.Id = villaFromDb.VillaDetails.Id;
                villa.VillaDetails.Villa = villa;
                villa.VillaDetails.CreatedDate = villaFromDb.VillaDetails.CreatedDate;

                _unitOfWork.Villas.Update(villa);
                await _unitOfWork.SaveChangesAsync();
                
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<VillaDto>(villa);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>{ex.ToString()};
            }
        
            return _response;
        }

        public async Task<ApiResponse> DeleteVillaAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("Id field is empty!");
                    return _response;
                }

                var villaSpecification = new VillaWithDetailsAndStatusSpecification(id);

                var villa = await _unitOfWork.Villas.FindSingle(villaSpecification);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Villa with id: ({id.ToString()} was not found)");
                    return _response;
                }
                
                await _unitOfWork.BeginTransactionAsync();

                _unitOfWork.Villas.Delete(villa);
                await _unitOfWork.SaveChangesAsync();

                //_unitOfWork.VillaDetails.Delete(villa.VillaDetails);
                //await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                var villaDto = _mapper.Map<VillaDto>(villa);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = villaDto;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = new List<string> { ex.ToString() };
                await _unitOfWork.RollbackTransactionAsync();
            }

            return _response;
        }
    }
}
