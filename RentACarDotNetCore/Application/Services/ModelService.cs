using AutoMapper;
using Domain.Entities;
using EmailService.Application.DTOs;
using MongoDB.Driver;
using RabbitMQ.Infrastructure.Abstract;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Model;
using RentACarDotNetCore.Application.Responses.Model;
using RentACarDotNetCore.Domain.Repositories;
using Serilog;
using UtilitiesClassLibrary.Exceptions;
using UtilitiesClassLibrary.Helpers;

namespace RentACarDotNetCore.Application.Services
{
	public class ModelService : IModelService
	{

		private readonly IMongoCollection<Model> _models;
		private readonly IMongoCollection<Brand> _brands;
		private readonly IMapper _mapper;
		private readonly IStringConverter _stringConverter;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IPublisher _publisher;
		public ModelService(IRentACarDatabaseSettings databaseSettings, IRedisCacheService redisCacheService, IMongoClient mongoClient, IMapper mapper, IStringConverter stringConverter, IPublisher publisher)
		{
			_mapper = mapper;
			var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
			_models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
			_brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
			_stringConverter = stringConverter;
			_redisCacheService = redisCacheService;
			_publisher = publisher;
		}

		public async Task<GetModelResponse> Get(string id)
		{
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			var cacheDataModels = await _redisCacheService.GetOrAddAsync("allmodels", async () => await _models.Find(model => true).ToListAsync());
			Model model = cacheDataModels.FirstOrDefault(model => model.Id == id);
			if (model != null && model.Brand != null)
			{
				model.Brand = cacheDataBrands.FirstOrDefault(brand => brand.Id == model.Brand.Id);
			}
			return _mapper.Map<GetModelResponse>(model);
		}

		public async Task<List<GetModelResponse>> Get()
		{
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			var cacheDataModels = await _redisCacheService.GetOrAddAsync("allmodels", async () => await _models.Find(model => true).ToListAsync());
			foreach (Model model in cacheDataModels)
			{
				if (model != null && model.Brand != null)
				{
					model.Brand = cacheDataBrands.FirstOrDefault(brand => brand.Id == model.Brand.Id);
				}
			}

			return _mapper.Map<List<GetModelResponse>>(cacheDataModels);
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

			Log.Warning("{@Model} is created", model, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<ModelDTO>("", "Add Model", "Model creation process attempted. Check result", _mapper.Map<ModelDTO>(model)));
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
			var existingModel = _models.Find(model => model.Id == id).FirstOrDefault();
			if (existingModel == null)
				throw new NotFoundException($"Model with id = {id} not found.");

			var result = _models.DeleteOne(model => model.Id == id);
			Log.Warning($"{id} is deleted", result, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<DeleteResult>("", "Delete Model", $"Model with id = {id} deletion process attempted.Check result !", result));

		}
	}
}
