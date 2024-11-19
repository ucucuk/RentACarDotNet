using RentACarDotNetCore.Application.DTOs;

namespace RentACarDotNetCore.Application.Requests
{
    public class UpdateModelRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BrandDTO BrandDTO { get; set; }
    }
}
