using RentACarDotNetCore.Application.Responses.User;
using RentACarDotNetCore.Domain.Entities;
using AutoMapper;

namespace RentACarDotNetCore.Utilities.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserResponse>().ReverseMap();

        }
    }
}
