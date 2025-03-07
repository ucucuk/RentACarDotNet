﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
	[Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
	public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }

        // GET: api/<ModelsController>
        [HttpGet]
		[Authorize(Roles = "admin,normal")]
		public async Task<ActionResult<List<GetModelResponse>>> Get()
        {
            return await _modelService.Get();
        }

		// GET: api/<ModelsController>
		[HttpGet("GetModelsByBrand")]
		[Authorize(Roles = "admin,normal")]
		public async Task<ActionResult<List<GetModelResponse>>> GetModelsByBrand(string brand)
		{
			return await _modelService.GetModelsByBrand(brand.ToUpper());
		}

		// GET api/<ModelsController>/5
		[HttpGet("{id}")]
		[Authorize(Roles = "admin,normal")]
		public async Task<ActionResult<GetModelResponse>> Get(string id)
        {
            GetModelResponse getModelResponse = await _modelService.Get(id);

            if (getModelResponse == null)
            {
                return NotFound($"Model with Id ={id} not found.");
            }

            return getModelResponse;
        }

        // POST api/<ModelsController>
        [HttpPost]
		[Authorize(Roles = "admin")]
		public ActionResult<ModelDTO> Post([FromBody] CreateModelRequest createModelRequest)
        {
            ModelDTO modelDTO = _modelService.Create(createModelRequest);
            return CreatedAtAction(nameof(Post), new { id = modelDTO.Id }, modelDTO);

        }

        // PUT api/<ModelsController>/5
        [HttpPut]
		[Authorize(Roles = "admin")]
		public ActionResult Put([FromBody] UpdateModelRequest updateModelRequest)
        {
            _modelService.Update(updateModelRequest);
            return NoContent();
        }

        // DELETE api/<ModelsController>/5
        [HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
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
