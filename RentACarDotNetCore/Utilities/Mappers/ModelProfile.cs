using AutoMapper;
using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Utilities.Mappers
{
    public class ModelProfile : Profile
    {
        public ModelProfile() 
        {
            
            CreateMap<Model, ModelDTO>().ReverseMap(); // soldakini sağa çeviriyor, ama kullanırken de tam tersi
            CreateMap<Model, UpdateModelRequest>().ReverseMap();
            CreateMap<Model, CreateModelRequest>().ReverseMap();
            CreateMap<Model, GetAllModelsResponse>()
              .ForMember(get => get.Brand, opt => opt.MapFrom(model => model.Brand)).ReverseMap(); 
            //formemberda da hedef solda kaynak sağda
            

        }

    }
}
