using AutoMapper;
using Domain.Entities;
using MongoDB.Driver;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Model;
using RentACarDotNetCore.Application.Responses.Model;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;
using RentACarDotNetCore.Utilities.Helpers;

namespace RentACarDotNetCore.Application.Services
{
    public class ModelService : IModelService
    {

        private readonly IMongoCollection<Model> _models;
        private readonly IMongoCollection<Brand> _brands;
        private readonly IMapper _mapper;
        private readonly IStringConverter _stringConverter;

        public ModelService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IMapper mapper, IStringConverter stringConverter)
        {
            _mapper = mapper;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
            _brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
            _stringConverter = stringConverter;
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
            //foreach (var model2 in models)
            //{
            //    model2.Name = _stringConverter.ConvertTRCharToENChar(model2.Name.ToUpper());
            //    _models.ReplaceOne(model => model.Id == model2.Id, model2);

            //}

            return _mapper.Map<List<GetModelResponse>>(models);
        }

        public ModelDTO Create(CreateModelRequest createModelRequest)
        {
            Brand brand = _brands.Find(brand => brand.Name.ToLower().Equals(createModelRequest.BrandName.ToLower())).FirstOrDefault();
            if (brand == null)
            {
                throw new NotFoundException($"{createModelRequest.BrandName} brand is not found.");
            }
            createModelRequest.Name = createModelRequest.Name.ToUpper();
            Model model = _mapper.Map<Model>(createModelRequest);
            model.Brand = brand;
            _models.InsertOne(model);
            return _mapper.Map<ModelDTO>(model);
        }

        public void Update(UpdateModelRequest updateModelRequest)
        {
            Brand brand = _brands.Find(brand => brand.Name.ToLower().Equals(updateModelRequest.Brand.Name.ToLower())).FirstOrDefault();
            if (brand == null)
            {
                throw new NotFoundException($"{updateModelRequest.Brand.Name} brand is not found.");
            }
            // var nameAndAgeFilter = Builders<Person>.Filter.And(
            //Builders<Person>.Filter.Eq(p => p.Name, "Alice"),
            //Builders<Person>.Filter.Gt(p => p.Age, 30)

            var nameFilter = Builders<Model>.Filter.Eq(model => model.Name, updateModelRequest.Name);
            var idFilter = Builders<Model>.Filter.Ne(model => model.Id, updateModelRequest.Id);
            var combineFilter = Builders<Model>.Filter.And(nameFilter, idFilter);
            var isDuplicated = _models.Find(combineFilter).FirstOrDefault();
            if (isDuplicated != null)
                throw new AlreadyExistsException($"{updateModelRequest.Name} model already exists.");

            var updateFilter = Builders<Model>.Filter.Eq(m => m.Id, updateModelRequest.Id);
            var model = _models.Find(updateFilter).FirstOrDefault();
            if (model == null)
                throw new NotFoundException($"Model with id = {updateModelRequest.Id} not found.");

            model.Name = updateModelRequest.Name;
            model.Brand = brand;
            _models.ReplaceOne(updateFilter, model);
            //Model model2 = _models.Find(model =>
            //(model.Name.ToLower().Equals(updateModelRequest.Name.ToLower()) && model.Id != updateModelRequest.Id)
            //).FirstOrDefault();
            //if (model2 != null)
            //{
            //    throw new AlreadyExistsException($"{updateModelRequest.Name} model already exists.");
            //}
            //Model model = _mapper.Map<Model>(updateModelRequest);
            //model.Brand = brand;
            //_models.ReplaceOne(model => model.Id == updateModelRequest.Id, model);
        }

        public void Delete(string id)
        {
            _models.DeleteOne(model => model.Id == id);
        }
    }
}
