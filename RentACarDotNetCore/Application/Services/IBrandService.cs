using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Brand;
using RentACarDotNetCore.Application.Responses.Brand;

namespace RentACarDotNetCore.Application.Services
{
    public interface IBrandService
    {
        List<GetBrandResponse> Get();
        List<GetBrandResponse> GetCache();
        Task<List<GetBrandWithModelsResponse>> GetBrandWithModels();
        Task<List<GetBrandWithModelsResponse>> GetBrandWithModelsCache();
        GetBrandResponse Get(string id);
        BrandDTO Create(CreateBrandRequest createBrandRequest);
        void Update(UpdateBrandRequest updateBrandRequest);
        void Delete(string id);

    }
}
