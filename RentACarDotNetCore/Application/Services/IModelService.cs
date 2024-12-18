using Domain.Entities;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Model;
using RentACarDotNetCore.Application.Responses.Model;

namespace RentACarDotNetCore.Application.Services
{
    public interface IModelService
    {
		Task<List<GetModelResponse>> Get();
		Task<GetModelResponse> Get(string id);
        ModelDTO Create(CreateModelRequest createModelRequest);
        void Update(UpdateModelRequest updateModelRequest);
        void Delete(string id);
    }
}
