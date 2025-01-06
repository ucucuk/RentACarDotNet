using AutoMapper;
using Domain.Entities;
using EmailService.Application.DTOs;
using MongoDB.Driver;
using RabbitMQ.Infrastructure.Abstract;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests;
using RentACarDotNetCore.Application.Responses;
using RentACarDotNetCore.Domain.Repositories;
using Serilog;
using UtilitiesClassLibrary.Exceptions;
using UtilitiesClassLibrary.Helpers;

namespace RentACarDotNetCore.Application.Services
{
	public class CarService : ICarService
	{
		private readonly IMongoCollection<Brand> _brands;
		private readonly IMongoCollection<Model> _models;
		private readonly IMongoCollection<Car> _cars;
		private readonly IMapper _mapper;
		private readonly IStringConverter _stringConverter;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IPublisher _publisher;

		public CarService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IRedisCacheService redisCacheService, IMapper mapper, IStringConverter stringConverter, IPublisher publisher)
		{

			_mapper = mapper;
			var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
			_brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
			_models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
			_cars = database.GetCollection<Car>(databaseSettings.CarsCollectionName);
			_stringConverter = stringConverter;
			_redisCacheService = redisCacheService;
			_publisher = publisher;
		}
		public async Task<List<GetCarResponse>> Get()
		{
			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			var cacheDataModels = await _redisCacheService.GetOrAddAsync("allmodels", async () => await _models.Find(model => true).ToListAsync());
			var cacheDataCars = await _redisCacheService.GetOrAddAsync("allcars", async () => await _cars.Find(car => true).ToListAsync());

			foreach (Car car in cacheDataCars)
			{
				if (car != null)
				{
					car.Model = cacheDataModels.Where(model => model.Id == car.Model.Id).FirstOrDefault();
					car.Model.Brand = cacheDataBrands.Where(brand => brand.Id == car.Model.Brand.Id).FirstOrDefault();
				}
			}
			watch.Stop();
			Console.WriteLine($"Uygulama Vakti GetBrandWithModelsResponse Cache: {watch.ElapsedMilliseconds} ms");
			return _mapper.Map<List<GetCarResponse>>(cacheDataCars);
		}

		public CarDTO Create(CreateCarRequest createCarRequest)
		{
			Car existsCar = _cars.Find(car => car.Plate.ToLower().Equals(createCarRequest.Plate.ToLower())).FirstOrDefault();
			if (existsCar != null)
				throw new AlreadyExistsException($"{createCarRequest.Plate} plate already exists.");

			Model model = _models.Find(model => model.Name.ToLower().Equals(createCarRequest.ModelName.ToLower())).FirstOrDefault();
			if (model == null)
				throw new NotFoundException($"{createCarRequest.ModelName} model is not found.");

			createCarRequest.Plate = _stringConverter.ConvertTRCharToENChar(createCarRequest.Plate.ToUpper());
			Car car = _mapper.Map<Car>(createCarRequest);
			car.Model = model;
			_cars.InsertOne(car);
			Log.Warning("{@Car} is created", car, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<CarDTO>("", "Add Car", "Car creation process attempted. Check result !", _mapper.Map<CarDTO>(car)));
			return _mapper.Map<CarDTO>(car);
		}

		public void Update(UpdateCarRequest updateCarRequest)
		{
			var updateFilter = Builders<Car>.Filter.Eq(m => m.Id, updateCarRequest.Id);
			var car = _cars.Find(updateFilter).FirstOrDefault();
			if (car == null)
				throw new NotFoundException($"Car with id = {updateCarRequest.Id} not found.");

			Model model = _models.Find(model => model.Name.ToLower().Equals(updateCarRequest.ModelName.ToLower())).FirstOrDefault();
			if (model == null)
				throw new NotFoundException($"{updateCarRequest.ModelName} model is not found.");


			// var nameAndAgeFilter = Builders<Person>.Filter.And(
			//Builders<Person>.Filter.Eq(p => p.Name, "Alice"),
			//Builders<Person>.Filter.Gt(p => p.Age, 30)

			var plateFilter = Builders<Car>.Filter.Eq(car => car.Plate, updateCarRequest.Plate);
			var idFilter = Builders<Car>.Filter.Ne(car => car.Id, updateCarRequest.Id);
			var combineFilter = Builders<Car>.Filter.And(plateFilter, idFilter);
			var isDuplicated = _cars.Find(combineFilter).FirstOrDefault();
			if (isDuplicated != null)
				throw new AlreadyExistsException($"{updateCarRequest.Plate} plate already exists.");


			car.Plate = updateCarRequest.Plate;
			car.Model = model;
			car.ModelYear = updateCarRequest.ModelYear;
			_cars.ReplaceOne(updateFilter, car);
		}
		public void Delete(string id)
		{
			var existingCar = _cars.Find(car => car.Id == id).FirstOrDefault();
			if (existingCar == null)
				throw new NotFoundException($"Car with id = {id} not found.");

			var result =_cars.DeleteOne(car => car.Id == id);
			Log.Warning($"{id} is deleted", result, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<DeleteResult>("", "Delete Car", $"Car with id = {id} deletion process attempted. Check result !", result));
		}


	}
}
