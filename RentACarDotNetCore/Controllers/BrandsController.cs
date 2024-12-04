using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Brand;
using RentACarDotNetCore.Application.Responses.Brand;
using RentACarDotNetCore.Application.Services;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        // GET: api/<BrandsController>
        [HttpGet]
        //[Authorize(Roles = "admin,normal")]
        public ActionResult<List<GetBrandResponse>> Get()
        {
            return _brandService.Get();
        }

        // GET: api/<BrandsController>
        [HttpGet("getbrandwithmodels")]
        //[Authorize(Roles = "admin")]
        public ActionResult<List<GetBrandWithModelsResponse>> GetBrandWithModels()
        {
            return _brandService.GetBrandWithModels();
        }

        // GET api/<BrandsController>/5
        [HttpGet("{id}")]
        public ActionResult<GetBrandResponse> Get(string id)
        {
            GetBrandResponse brand = _brandService.Get(id);
            if (brand == null)
            {
                return NotFound($"Brand with Id = {id} not found.");
            }
            return brand;
        }

        // POST api/<BrandsController>
        [HttpPost]
        public ActionResult<BrandDTO> Post([FromBody] CreateBrandRequest createBrandRequest)
        {
            BrandDTO brandDTO = _brandService.Create(createBrandRequest);
            return CreatedAtAction(nameof(Post), new { id = brandDTO.Id }, brandDTO);
        }

        // PUT api/<BrandsController>/5
        [HttpPut]
        public ActionResult Put([FromBody] UpdateBrandRequest updateBrandRequest)
        {
            var existingBrand = _brandService.Get(updateBrandRequest.Id);

            if (existingBrand == null)
            {
                return NotFound($"Brand with id = {updateBrandRequest.Id} not found.");
            }
            _brandService.Update(updateBrandRequest);
            return NoContent();
        }

        // DELETE api/<BrandsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var existingBrand = _brandService.Get(id);

            if (existingBrand == null)
            {
                return NotFound($"Brand with id = {id} not found.");
            }

            _brandService.Delete(id);

            return Ok($"Brand with id = {id} deleted.");
        }
    }
}
