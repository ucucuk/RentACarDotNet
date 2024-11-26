using RentACarDotNetCore.Application.DTOs;

namespace RentACarDotNetCore.Application.Responses.Brand
{
    public class GetBrandWithModelsResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ModelIdNameDTO> Models { get; set; }
    }
}
