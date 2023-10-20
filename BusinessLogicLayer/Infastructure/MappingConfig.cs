using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Dto.Order;
using BusinessLogicLayer.Dto.User;
using BusinessLogicLayer.Dto.Villa;
using DataLayer.Models;

namespace BusinessLogicLayer.Infastructure
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            VillaMapConfiguration();
            UserMapConfiguration();
        }

        private void VillaMapConfiguration()
        {
            CreateMap<Villa, VillaPartialDto>();

            CreateMap<Villa, VillaDto>()
                .ForMember(dest => dest.Price, opt =>
                    opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Rate, opt =>
                    opt.MapFrom(src => src.VillaDetails.Rate))
                .ForMember(dest => dest.Sqmt, opt =>
                    opt.MapFrom(src => src.VillaDetails.Sqmt))
                .ForMember(dest => dest.Occupancy, opt =>
                    opt.MapFrom(src => src.VillaDetails.Occupancy))
                .ForMember(dest => dest.VillaStatus, opt =>
                    opt.MapFrom(src => src.Status.Status));

            CreateMap<VillaCreateDto, Villa>()
                .ForMember(dest => dest.VillaDetails, opt =>
                    opt.MapFrom((dto, villa) => new VillaDetails()
                    {
                        Rate = dto.Rate,
                        Sqmt = dto.Sqmt,
                        Occupancy = dto.Occupancy,
                        Villa = villa,
                    }));

            CreateMap<VillaUpdateDto, Villa>()
                .ForMember(dest => dest.VillaDetails, opt =>
                    opt.MapFrom((dto, villa) => new VillaDetails()
                    {
                        //Id = dto.VillaDetailsId,
                        Rate = dto.Rate,
                        Sqmt = dto.Sqmt,
                        Occupancy = dto.Occupancy,
                        Villa = villa,
                    }))
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<VillaStatusDto, VillaStatus>().ReverseMap();

            //CreateMap<VillaCreateDto, Villa>()
            //    .ForMember(dest => dest.VillaDetails, opt => opt.MapFrom(s => s));

        }

        private void UserMapConfiguration()
        {
            CreateMap<RegistrationDto, Users>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Users, AuthUserDto>()
                .ForMember(dist => dist.Role, opt => 
                    opt.MapFrom(src => src.Role.RoleName));

            CreateMap<Users, UserPartialDto>()
                .ForMember(dest => dest.Role, opt =>
                    opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.OrdersCount, opt =>
                    opt.MapFrom(src => src.Orders.Count));

        }

        private void OrderMapConfiguration()
        {
            //CreateMap<Orders,OrderPartialDto>()
            //    .ForMember(dest => dest.Status, opt => 
            //        opt.MapFrom(src => src.Status.Status))
            //    .ForMember();
        }
    }   
}
