using RentACarDotNetCore.Application.DTOs;

namespace RentACarDotNetCore.Application.Responses
{
    public class GetCarResponse
    {
        public string Id { get; set; }

        public ModelDTO Model { get; set; }

        public string Plate { get; set; }

        public int ModelYear { get; set; }

  

        

    }
}
