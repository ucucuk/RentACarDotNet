using AutoMapper;
using Domain.Entities;

namespace Utilities.Mappers
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Brand, CreateBrandRequest>().ReverseMap();
            CreateMap<Brand, UpdateBrandRequest>().ReverseMap();
            CreateMap<Brand, GetBrandResponse>().ReverseMap();
            CreateMap<Brand, GetBrandWithModelsResponse>();
                //.ForMember(get => get.Models, opt => opt.MapFrom(brand => brand.Models));
            //.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        }

    }
}
