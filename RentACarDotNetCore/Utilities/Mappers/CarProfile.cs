﻿using AutoMapper;
using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

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
