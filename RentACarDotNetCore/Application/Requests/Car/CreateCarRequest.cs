using Domain.Entities;

namespace RentACarDotNetCore.Application.Requests
{
    public class CreateCarRequest
    {
        public string ModelName { get; set; }

        public string Plate { get; set; }

        public int ModelYear { get; set; }
    }
}
