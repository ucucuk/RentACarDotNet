using AutoMapper;
using Domain.Entities;
using MongoDB.Driver;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;

namespace RentACarDotNetCore.Application.Services
{
    public class ModelService : IModelService
    {

        private readonly IMongoCollection<Model> _models;
        private readonly IMongoCollection<Brand> _brands;
        private readonly IMapper _mapper;

        public ModelService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IMapper mapper)
        {
            _mapper = mapper;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
            _brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
        }

        public GetModelResponse Get(string id)
        {
            Model model = _models.Find(model => model.Id == id).FirstOrDefault();
            if (model != null && model.Brand != null)
            {
                model.Brand = _brands.Find(brand => brand.Id == model.Brand.Id).FirstOrDefault();
            }
            return _mapper.Map<GetModelResponse>(model);
        }

        public List<GetModelResponse> Get()
        {
            List<Model> models = _models.Find(model => true).ToList();
            foreach (Model model in models)
            {
                if (model != null && model.Brand != null)
                {
                    model.Brand = _brands.Find(brand => brand.Id == model.Brand.Id).FirstOrDefault();
                }
            }
            return _mapper.Map<List<GetModelResponse>>(models);
        }

        public Model Create(CreateModelRequest createModelRequest)
        {
            Brand brand = _brands.Find(brand => brand.Name.Equals(createModelRequest.BrandName)).FirstOrDefault();
            if (brand == null)
            {
                throw new NotFoundException($"{createModelRequest.BrandName} brand is not found.");
            }
            Model model = _mapper.Map<Model>(createModelRequest);
            model.Brand = brand;
            _models.InsertOne(model);
            return model;
        }

        public void Update(UpdateModelRequest updateModelRequest)
        {
            _models.ReplaceOne(model => model.Id == updateModelRequest.Id, _mapper.Map<Model>(updateModelRequest));
        }

        public void Delete(string id)
        {
            _models.DeleteOne(model => model.Id == id);
        }
    }
}
