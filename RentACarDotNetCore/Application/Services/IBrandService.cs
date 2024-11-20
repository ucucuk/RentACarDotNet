using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Application.Services
{
    public interface IBrandService
    {
        List<GetBrandResponse> Get();
        List<GetBrandWithModelsResponse> GetBrandWithModels();
        Brand Get(string id);
        BrandDTO Create(CreateBrandRequest createBrandRequest);
        void Update(UpdateBrandRequest updateBrandRequest);
        void Delete(string id);

    }
}
