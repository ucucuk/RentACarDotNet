using Application.DTOs;
using Application.Requests.Car;
using Application.Responses.Car;
using AutoMapper;
using Domain.Entities;

namespace RentACarDotNetCore.Utilities.Mappers
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {

            CreateMap<Car, CarDTO>().ReverseMap(); // soldakini sağa çeviriyor, ama kullanırken de tam tersi
            CreateMap<Car, UpdateCarRequest>().ReverseMap();
            CreateMap<Car, CreateCarRequest>().ReverseMap();
            CreateMap<Car, GetCarResponse>().ReverseMap();
            //.ForMember(get => get.Model, opt => opt.MapFrom(car => car.Model))


            //formemberda da hedef solda kaynak sağda



        }

    }
}
