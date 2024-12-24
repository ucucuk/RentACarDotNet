using Application.DTOs;
using Application.Requests.Brand;
using Application.Responses.Brand;
using Domain.Entities;

namespace Application.Services
{
	public interface IBrandService
	{

		void SendMailFromRabbitMQ();
		Task<List<GetBrandResponse>> Get();
		Task<List<GetBrandWithModelsResponse>> GetBrandWithModels();
		Task<GetBrandResponse> Get(string id);
		BrandDTO Create(CreateBrandRequest createBrandRequest);
		void Update(UpdateBrandRequest updateBrandRequest);
		void Delete(string id);

	}
}
