using Domain.Entities;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Application.Services
{
    public interface IModelService
    {
        List<GetModelResponse> Get();
        GetModelResponse Get(string id);
        Model Create(CreateModelRequest createModelRequest);
        void Update(UpdateModelRequest updateModelRequest);
        void Delete(string id);
    }
}
