using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }


        // GET: api/<ModelsController>
        [HttpGet]
        public ActionResult<List<GetAllModelsResponse>> Get()
        {
            return _modelService.Get();
        }

        // GET api/<ModelsController>/5
        [HttpGet("{id}")]
        public ActionResult<Model> Get(string id)
        {
            var model = _modelService.Get(id);

            if (model == null)
            {
                return NotFound($"Model with Id ={id} not found.");
            }

            return model;
        }

        // POST api/<ModelsController>
        [HttpPost]
        public ActionResult<Model> Post([FromBody] CreateModelRequest createModelRequest)
        {
            Model model = _modelService.Create(createModelRequest);
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);

        }

        // PUT api/<ModelsController>/5
        [HttpPut()]
        public ActionResult Put([FromBody] UpdateModelRequest updateModelRequest)
        {
            var existingModel = _modelService.Get(updateModelRequest.Id);
            if (existingModel == null)
            {
                NotFound($"Model with id = {updateModelRequest.Id} not found.");
            }
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
