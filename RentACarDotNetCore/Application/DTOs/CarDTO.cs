namespace RentACarDotNetCore.Application.DTOs
{
    public class CarDTO
    {
        public string Id { get; set; }

        public ModelDTO Model { get; set; }

        public string Plate { get; set; }

        public int ModelYear { get; set; }
    }
}
