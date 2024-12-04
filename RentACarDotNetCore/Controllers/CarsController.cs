using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/<CarsController>
        [HttpGet]
        public ActionResult<List<GetCarResponse>> Get()
        {
            return _carService.Get();
        }

        // POST api/<CarsController>
        [HttpPost]
        public ActionResult<CarDTO> Post([FromBody] CreateCarRequest createCarRequest)
        {
            CarDTO carDTO = _carService.Create(createCarRequest);
            return CreatedAtAction(nameof(Post), new { id = carDTO.Id }, carDTO);

        }

        // PUT api/<CarsController>/5
        [HttpPut]
        public ActionResult Put([FromBody] UpdateCarRequest updateCarRequest)
        {
            _carService.Update(updateCarRequest);
            return NoContent();
        }

        // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            _carService.Delete(id);

            return Ok($"Car with id = {id} deleted.");
        }
    }
}
