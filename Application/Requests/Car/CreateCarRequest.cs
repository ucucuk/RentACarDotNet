﻿namespace Application.Requests.Car
{
	public class CreateCarRequest
	{
		public string ModelName { get; set; }

		public string Plate { get; set; }

		public int ModelYear { get; set; }
	}
}