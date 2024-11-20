using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;

namespace RentACarDotNetCore.Application.Services
{
    public interface IModelService
    {
        List<GetModelResponse> Get();
        GetModelResponse Get(string id);
        ModelDTO Create(CreateModelRequest createModelRequest);
        void Update(UpdateModelRequest updateModelRequest);
        void Delete(string id);
    }
}
