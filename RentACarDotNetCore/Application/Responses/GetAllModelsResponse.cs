namespace RentACarDotNetCore.Application.Responses
{
    public class GetAllModelsResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public GetAllBrandsResponse Brand { get; set; }
    }
}
