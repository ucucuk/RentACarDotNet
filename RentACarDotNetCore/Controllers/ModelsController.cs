using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Model;
using RentACarDotNetCore.Application.Responses.Model;
using RentACarDotNetCore.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }


        // GET: api/<ModelsController>
        [HttpGet]
        public ActionResult<List<GetModelResponse>> Get()
        {
            return _modelService.Get();
        }

        // GET api/<ModelsController>/5
        [HttpGet("{id}")]
        public ActionResult<GetModelResponse> Get(string id)
        {
            GetModelResponse getModelResponse = _modelService.Get(id);

            if (getModelResponse == null)
            {
                return NotFound($"Model with Id ={id} not found.");
            }

            return getModelResponse;
        }

        // POST api/<ModelsController>
        [HttpPost]
        public ActionResult<ModelDTO> Post([FromBody] CreateModelRequest createModelRequest)
        {
            ModelDTO modelDTO = _modelService.Create(createModelRequest);
            return CreatedAtAction(nameof(Post), new { id = modelDTO.Id }, modelDTO);

        }

        // PUT api/<ModelsController>/5
        [HttpPut]
        public ActionResult Put([FromBody] UpdateModelRequest updateModelRequest)
        {
            _modelService.Update(updateModelRequest);
            return NoContent();
        }

        // DELETE api/<ModelsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var existingModel = _modelService.Get(id);

            if (existingModel == null)
            {
                NotFound($"Model with id = {id} not found.");
            }

            _modelService.Delete(id);

            return Ok($"Model with id = {id} deleted.");
        }
    }
}
