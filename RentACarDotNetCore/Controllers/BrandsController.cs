﻿using Domain.Entities;
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
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        // GET: api/<BrandsController>
        [HttpGet]
        public ActionResult<List<GetAllBrandsResponse>> Get()
        {
            return _brandService.Get();
        }

        // GET api/<BrandsController>/5
        [HttpGet("{id}")]
        public ActionResult<Brand> Get(string id)
        {
            var brand = _brandService.Get(id);
            if (brand == null)
            {
                return NotFound($"Brand with Id = {id} not found.");
            }
            return brand;
        }

        // POST api/<BrandsController>
        [HttpPost]
        public ActionResult<Brand> Post([FromBody] BrandDTO brandDTO)
        {
            Brand brand = _brandService.Create(brandDTO);
            return CreatedAtAction(nameof(Get), new { id = brand.Id }, brand);
        }

        // PUT api/<BrandsController>/5
        [HttpPut()]
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