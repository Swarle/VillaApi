﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dto;
using DataLayer.Models;

namespace BusinessLogicLayer.Infastructure
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            VillaMapConfiguration();
        }

        private void VillaMapConfiguration()
        {
            CreateMap<Villa, VillaPartialDto>();

            CreateMap<Villa, VillaDto>()
                .ForMember(dest => dest.VillaDetailsId, opt => 
                    opt.MapFrom(src => src.VillaDetails.Id))
                .ForMember(dest => dest.Price, opt =>
                    opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.VillaStatusId, opt =>
                    opt.MapFrom(src => src.Status.Id))
                .ForMember(dest => dest.Rate, opt =>
                    opt.MapFrom(src => src.VillaDetails.Rate))
                .ForMember(dest => dest.Sqmt, opt =>
                    opt.MapFrom(src => src.VillaDetails.Sqmt))
                .ForMember(dest => dest.Occupancy, opt =>
                    opt.MapFrom(src => src.VillaDetails.Occupancy))
                .ForMember(dest => dest.VillaStatus, opt =>
                    opt.MapFrom(src => src.Status.Status));

        }
    }   
}
