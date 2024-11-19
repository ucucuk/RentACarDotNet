using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Application.Services
{
    public interface IBrandService
    {
        List<GetAllBrandsResponse> Get();
        Brand Get(string id);
        Brand Create(BrandDTO brandDTO);
        void Update(UpdateBrandRequest updateBrandRequest);
        void Delete(string id);

    }
}
