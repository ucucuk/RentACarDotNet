using Application.DTOs;
using Application.Requests.Model;
using Application.Responses.Model;
using AutoMapper;
using Domain.Entities;

namespace RentACarDotNetCore.Utilities.Mappers
{
	public class ModelProfile : Profile
    {
        public ModelProfile() 
        {
            
            CreateMap<Model, ModelDTO>().ReverseMap(); // soldakini sağa çeviriyor, ama kullanırken de tam tersi
            CreateMap<Model, UpdateModelRequest>().ReverseMap();
            CreateMap<Model, CreateModelRequest>().ReverseMap();
            CreateMap<Model, ModelIdNameDTO>().ReverseMap();
            CreateMap<Model, GetModelResponse>().ReverseMap(); 
            //.ForMember(get => get.Brand, opt => opt.MapFrom(model => model.Brand))
            //formemberda da hedef solda kaynak sağda


        }

    }
}
