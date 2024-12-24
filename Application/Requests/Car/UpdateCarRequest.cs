namespace Application.Requests.Car
{
	public class UpdateCarRequest
	{
		public string Id { get; set; }

		public string ModelName { get; set; }

		public string Plate { get; set; }

		public int ModelYear { get; set; }
	}
}
