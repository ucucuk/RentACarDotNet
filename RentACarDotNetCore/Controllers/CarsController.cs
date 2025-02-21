using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Application.Responses.Model;
using RentACarDotNetCore.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	[Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]

	public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/<CarsController>
        [HttpGet]
		[Authorize(Roles = "admin,normal")]
		public async Task<ActionResult<List<GetCarResponse>>> Get()
        {
            return await _carService.Get();
        }

		// GET: api/<ModelsController>
		[HttpGet("GetCarsByModel")]
		[Authorize(Roles = "admin,normal")]
		public async Task<ActionResult<List<GetCarResponse>>> GetCarsByModel(string model)
		{
			return await _carService.GetCarsByModel(model.ToUpper());
		}
		// POST api/<CarsController>
		[HttpPost]
		[Authorize(Roles = "admin")]
		public ActionResult<CarDTO> Post([FromBody] CreateCarRequest createCarRequest)
        {
            CarDTO carDTO = _carService.Create(createCarRequest);
            return CreatedAtAction(nameof(Post), new { id = carDTO.Id }, carDTO);

        }

        // PUT api/<CarsController>/5
        [HttpPut]
		[Authorize(Roles = "admin")]
		public ActionResult Put([FromBody] UpdateCarRequest updateCarRequest)
        {
            _carService.Update(updateCarRequest);
            return NoContent();
        }

        // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
		public ActionResult Delete(string id)
        {
            _carService.Delete(id);

            return Ok($"Car with id = {id} deleted.");
        }
    }
}
