using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Application.Services
{
    public interface IBrandService
    {
        List<GetBrandResponse> Get();
        Brand Get(string id);
        Brand Create(CreateBrandRequest createBrandRequest);
        void Update(UpdateBrandRequest updateBrandRequest);
        void Delete(string id);

    }
}
