using Application.DTOs;
using Application.Requests.Car;
using Application.Responses.Car;
using Domain.Entities;

namespace Application.Services
{
	public interface ICarService
	{
		Task<List<GetCarResponse>> Get();

		CarDTO Create(CreateCarRequest createCarRequest);
		void Update(UpdateCarRequest updateCarRequest);
		void Delete(string id);

	}
}
