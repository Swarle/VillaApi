using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
using BusinessLogicLayer.Dto.Villa;
using BusinessLogicLayer.Infastructure;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApiResponse _response;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUriService _uriService;

        public VillaService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,IUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
            _httpContextAccessor = httpContextAccessor;
            _uriService = uriService;
        }

        public async Task<ApiResponse> GetVillasPartialAsync(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var paginationSpecification = new PagingSpecification((validFilter.PageNumber - 1), validFilter.PageSize);

                var villaPaginationSpecification = new VillaPaginationSpecification(paginationSpecification);

                var villas = await _unitOfWork.Villas.Find(villaPaginationSpecification);

                if (!villas.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("No Villas were found!");
                    return _response;
                }

                var villasPartialDto = _mapper.Map<List<VillaPartialDto>>(villas);

                var route = _httpContextAccessor.HttpContext.Request.Path.Value;

                var totalRecords = (await _unitOfWork.Villas.GetAllAsync()).Count();

                var pagedResponse = PaginationHelper.CreatePagedResponse<VillaPartialDto>(villasPartialDto, filter,
                    totalRecords, _uriService, route);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = pagedResponse;
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
                ISpecification<Villa> specification = new FindVillaWithDetailsAndStatusSpecification();

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
                    _response.ErrorMessage.Add("Id is empty!");
                    return _response;
                }

                var specification = new FindVillaWithDetailsAndStatusSpecification(id);
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

        public async Task<ApiResponse> CreateVillaAsync(VillaCreateDto createDto)
        {
            try
            {
                var villaSpecification = new FindVillaSpecification(createDto.Name, createDto.VillaNumber);

                var isExist = await _unitOfWork.Villas.FindSingle(villaSpecification);

                if (isExist != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessage.Add("Villa already exists!");
                    return _response;
                }

                var villa = _mapper.Map<Villa>(createDto);

                var villaStatusSpecification = new FindVillaStatusSpecification(VillaStatusSD.Available);

                var status = await _unitOfWork.VillaStatus.FindSingle(villaStatusSpecification);

                if (status == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessage.Add($"Status {VillaStatusSD.Available} does not exist!");
                    return _response;
                }

                villa.Status = status;
                villa.StatusId = status.Id;
                villa.Id = Guid.NewGuid();

                if (createDto.Image != null)
                {
                    var fileName = villa.Id + Path.GetExtension(createDto.Image.FileName);
                    var filePath = @"wwwroot\VillaImages\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    
                    var baseUrl =
                        $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}{_httpContextAccessor.HttpContext.Request.PathBase.Value}";
                    createDto.ImageUrl = baseUrl + "/VillaImages/" + fileName;
                    createDto.ImageLocalPath = filePath;

                    await using var fileStream = new FileStream(directoryLocation, FileMode.Create);
                    await createDto.Image.CopyToAsync(fileStream);
                }
                else
                {
                    createDto.ImageUrl = "https://placehold.co/600x400";
                }

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
                var villaSpecification = new FindVillaWithDetailsAndStatusSpecification(updateDto.Id, true);

                var villaFromDb = await _unitOfWork.Villas.FindSingle(villaSpecification);

                if (villaFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("Villa does not exist!");
                    return _response;
                }
                
                var status = await _unitOfWork.VillaStatus.GetByIdAsync(updateDto.VillaStatusId);

                if (status == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Status {updateDto.VillaStatusId} does not exist!");
                    return _response;
                }

                var villa = _mapper.Map<Villa>(updateDto);

                villa.StatusId = status.Id;
                villa.Status = status;
                villa.VillaDetails.Id = villaFromDb.VillaDetails.Id;
                villa.VillaDetails.Villa = villa;
                villa.VillaDetails.CreatedDate = villaFromDb.VillaDetails.CreatedDate;

                if (updateDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(villaFromDb.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villaFromDb.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);

                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    var fileName = villa.Id + Path.GetExtension(updateDto.Image.FileName);
                    var filePath = @"wwwroot\VillaImages\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    var baseUrl =
                        $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}{_httpContextAccessor.HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/VillaImages/" + fileName;
                    villa.ImageLocalPath = filePath;

                    await using var fileStream = new FileStream(directoryLocation, FileMode.Create);
                    await updateDto.Image.CopyToAsync(fileStream);
                }
                else
                {
                    villa.ImageUrl = villaFromDb.ImageUrl;
                    villa.ImageLocalPath = villaFromDb.ImageLocalPath;
                }

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

                var villaSpecification = new FindVillaWithDetailsAndStatusSpecification(id);

                var villa = await _unitOfWork.Villas.FindSingle(villaSpecification);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add($"Villa with id:({id}) was not found");
                    return _response;
                }

                _unitOfWork.Villas.Delete(villa);
                await _unitOfWork.SaveChangesAsync();

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

        public async Task<ApiResponse> GetVillaStatusesAsync()
        {
            try
            {
                var villaStatuses = await _unitOfWork.VillaStatus.GetAllAsync();

                if (!villaStatuses.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage.Add("Statuses not found!");
                    return _response;
                }

                var statusDtoList = _mapper.Map<List<VillaStatusDto>>(villaStatuses);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = statusDtoList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }
    }
}
