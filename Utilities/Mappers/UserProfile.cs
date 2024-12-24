using RentACarDotNetCore.Domain.Entities;
using AutoMapper;
using Application.Responses.User;

namespace Utilities.Mappers
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<User, GetUserResponse>().ReverseMap();

		}
	}
}
