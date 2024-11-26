using Domain.Entities;

namespace RentACarDotNetCore.Application.Requests
{
    public class UpdateCarRequest
    {
        public string Id { get; set; }

        public string ModelName { get; set; }

        public string Plate { get; set; }

        public int ModelYear { get; set; }
    }
}
