using AutoMapper;
using Domain.Entities;
using MongoDB.Driver;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;
using System.Collections.Generic;

namespace RentACarDotNetCore.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Brand> _brands;
        private readonly IMapper _mapper;

        public BrandService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IMapper mapper)
        {
            _mapper = mapper;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
        }

        public Brand Get(string id)
        {
            Brand existsBrand = _brands.Find(brand => brand.Id == id).FirstOrDefault();
            if (existsBrand == null)
            {
                throw new NotFoundException($"No brand found with this {id}");
            }
            return _brands.Find(brand => brand.Id == id).FirstOrDefault();
        }

        public List<GetBrandResponse> Get()
        {
            return _mapper.Map<List<GetBrandResponse>>(_brands.Find(brand => true).ToList());
        }

        public Brand Create(CreateBrandRequest createBrandRequest)
        {
            Brand existsBrand = _brands.Find(brand => brand.Name.ToLower().Equals(createBrandRequest.Name.ToLower())).FirstOrDefault();
            if (existsBrand != null)
            {
                throw new AlreadyExistsException($"{createBrandRequest.Name} brand already exists.");
            }
            Brand brand = _mapper.Map<Brand>(createBrandRequest);
            _brands.InsertOne(brand);
            return brand;
        }

        public void Update(UpdateBrandRequest updateBrandRequest)
        {
            Brand existsBrand = _brands.Find(brand => brand.Name.ToLower().Equals(updateBrandRequest.Name.ToLower())).FirstOrDefault();
            if (existsBrand != null)
            {
                throw new AlreadyExistsException($"{updateBrandRequest.Name} brand already exists.");
            }
            _brands.ReplaceOne(brand => brand.Id == updateBrandRequest.Id, _mapper.Map<Brand>(updateBrandRequest));
        }

        public void Delete(string id)
        {
            _brands.DeleteOne(brand => brand.Id == id);
        }





    }
}
