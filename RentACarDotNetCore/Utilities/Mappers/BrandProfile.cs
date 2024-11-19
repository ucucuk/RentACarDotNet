using AutoMapper;
using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Utilities.Mappers
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Brand, UpdateBrandRequest>().ReverseMap();
            CreateMap<Brand, GetBrandResponse>().ReverseMap();
            //.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        }

    }
}
