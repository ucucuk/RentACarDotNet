using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Application.Services
{
    public interface ICarService
    {
        List<GetCarResponse> Get();

        CarDTO Create(CreateCarRequest createCarRequest);
        void Update(UpdateCarRequest updateCarRequest);
        void Delete(string id);

    }
}
