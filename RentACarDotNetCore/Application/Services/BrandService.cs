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
            Brand existsBrand = _brands.Find(brand => brand.Name.Equals(createBrandRequest.Name)).FirstOrDefault();
            if (existsBrand != null)
            {
                throw new AlreadyExistsException($"{createBrandRequest.Name} brand is already exists.");
            }
            Brand brand = _mapper.Map<Brand>(createBrandRequest);
            _brands.InsertOne(brand);
            return brand;
        }

        public void Update(UpdateBrandRequest updateBrandRequest)
        {
            _brands.ReplaceOne(brand => brand.Id == updateBrandRequest.Id, _mapper.Map<Brand>(updateBrandRequest));
        }

        public void Delete(string id)
        {
            _brands.DeleteOne(brand => brand.Id == id);
        }





    }
}
