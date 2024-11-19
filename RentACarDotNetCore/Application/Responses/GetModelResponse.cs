namespace RentACarDotNetCore.Application.Responses
{
    public class GetModelResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public GetBrandResponse Brand { get; set; }
    }
}
