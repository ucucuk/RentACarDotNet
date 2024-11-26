using RentACarDotNetCore.Application.Responses.Brand;

namespace RentACarDotNetCore.Application.Responses.Model
{
    public class GetModelResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public GetBrandResponse Brand { get; set; }
    }
}
