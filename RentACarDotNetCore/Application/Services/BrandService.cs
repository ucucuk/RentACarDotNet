using AutoMapper;
using Domain.Entities;
using MongoDB.Driver;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;
using RentACarDotNetCore.Utilities.Helpers;

namespace RentACarDotNetCore.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Brand> _brands;
        private readonly IMongoCollection<Model> _models;
        private readonly IMapper _mapper;
        private readonly IStringConverter _stringConverter;


        public BrandService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IMapper mapper, IStringConverter stringConverter)
        {
            _mapper = mapper;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
            _models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
            _stringConverter = stringConverter;
        }

        public GetBrandResponse Get(string id)
        {
            Brand existsBrand = _brands.Find(brand => brand.Id == id).FirstOrDefault();
            if (existsBrand == null)
            {
                throw new NotFoundException($"No brand found with this {id}");
            }
            return _mapper.Map<GetBrandResponse>(_brands.Find(brand => brand.Id == id).FirstOrDefault());
        }

        public List<GetBrandResponse> Get()
        {
            return _mapper.Map<List<GetBrandResponse>>(_brands.Find(brand => true).ToList());
        }

        public List<GetBrandWithModelsResponse> GetBrandWithModels()
        {
            List<Brand> brands = _brands.Find(brand => true).ToList();
            foreach (Brand brand in brands)
            {
                brand.Models = _models.Find(model => model.Brand.Id == brand.Id).ToList();
            }
            return _mapper.Map<List<GetBrandWithModelsResponse>>(brands);
        }

        public BrandDTO Create(CreateBrandRequest createBrandRequest)
        {
            Brand existsBrand = _brands.Find(brand => brand.Name.ToLower().Equals(createBrandRequest.Name.ToLower())).FirstOrDefault();
            if (existsBrand != null)
            {
                throw new AlreadyExistsException($"{createBrandRequest.Name} brand already exists.");
            }
            createBrandRequest.Name = _stringConverter.ConvertTRCharToENChar(createBrandRequest.Name.ToUpper());
            Brand brand = _mapper.Map<Brand>(createBrandRequest);
            _brands.InsertOne(brand);
            return _mapper.Map<BrandDTO>(brand);
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
