using Application.DTOs;

namespace Application.Requests.Model
{
	public class UpdateModelRequest
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public BrandDTO Brand { get; set; }
	}
}
