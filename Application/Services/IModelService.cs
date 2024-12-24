using Application.DTOs;
using Application.Requests.Model;
using Application.Responses.Model;
using Domain.Entities;

namespace Application.Services
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
