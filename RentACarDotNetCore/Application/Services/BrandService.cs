using AutoMapper;
using Domain.Entities;
using EmailService.Application.Abstarct;
using EmailService.Application.DTOs;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RabbitMQ.Infrastructure.Abstract;
using RabbitMQ.Infrastructure.Concrete;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Brand;
using RentACarDotNetCore.Application.Responses.Brand;
using RentACarDotNetCore.Controllers;
using RentACarDotNetCore.Domain.Repositories;
using Serilog;
using System.Text.Json;
using UtilitiesClassLibrary.Exceptions;
using UtilitiesClassLibrary.Helpers;

namespace RentACarDotNetCore.Application.Services
{
	public class BrandService : IBrandService
	{
		private readonly IMongoCollection<Brand> _brands;
		private readonly IMongoCollection<Model> _models;
		private readonly IMapper _mapper;
		private readonly IStringConverter _stringConverter;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IPublisher _publisher;
		public BrandService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IRedisCacheService redisCacheService,
			IMapper mapper, IStringConverter stringConverter, IPublisher publisher, IMailService mailService, ILogger<BrandService> logger)
		{
			var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
			_brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
			_models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
			_stringConverter = stringConverter;
			_mapper = mapper;
			_redisCacheService = redisCacheService;
			_publisher = publisher;
		}

		public async Task<GetBrandResponse> Get(string id)
		{
			Brand existsBrand = new Brand();
			//var cacheData = _redisCacheService.GetOrAdd("allbrands", () => { return _brands.Find(brand => true).ToList(); });
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			if (cacheDataBrands != null)
			{
				existsBrand = cacheDataBrands.FirstOrDefault(brand => brand.Id == id);
				//existsBrand = cacheData.Where(b => b.Id == id).FirstOrDefault();
			}
			if (existsBrand == null)
			{
				existsBrand = _brands.Find(brand => brand.Id == id).FirstOrDefault();
			}

			if (existsBrand == null)
				throw new NotFoundException($"No brand found with this {id}");

			return _mapper.Map<GetBrandResponse>(existsBrand);

		}

		public async Task<List<GetBrandResponse>> Get()
		{
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			if (cacheDataBrands.Count == 0)
			{
				cacheDataBrands = _brands.Find(brand => true).ToList();
			}
			return _mapper.Map<List<GetBrandResponse>>(cacheDataBrands);
		}


		public async Task<List<GetBrandWithModelsResponse>> GetBrandWithModels()
		{
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			var cacheDataModels = await _redisCacheService.GetOrAddAsync("allmodels", async () => await _models.Find(model => true).ToListAsync());
			foreach (Brand brand in cacheDataBrands)
			{
				brand.Models = cacheDataModels.Where(model => model.Brand.Id == brand.Id).ToList();
			}
			return _mapper.Map<List<GetBrandWithModelsResponse>>(cacheDataBrands);
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

			Log.Warning("{@Brand} is created", brand, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<BrandDTO>("", "Add Brand", "Brand creation process attempted. Check result", _mapper.Map<BrandDTO>(brand)));
			return _mapper.Map<BrandDTO>(brand);
		}

		public void Update(UpdateBrandRequest updateBrandRequest)
		{
			Brand existsBrand = _brands.Find(brand => brand.Name.ToLower().Equals(updateBrandRequest.Name.ToLower())).FirstOrDefault();
			if (existsBrand != null)
			{
				throw new AlreadyExistsException($"{updateBrandRequest.Name} brand already exists.");
			}
			Brand updBrand = _brands.Find(brand => brand.Id.Equals(updateBrandRequest.Id)).FirstOrDefault();

			updBrand.Name= updateBrandRequest.Name.ToUpper();
			_brands.ReplaceOne(brand => brand.Id == updateBrandRequest.Id, updBrand);
		}

		public void Delete(string id)
		{
			var existingBrand = _brands.Find(brand => brand.Id == id).FirstOrDefault();
			if (existingBrand == null)
				throw new NotFoundException($"Brand with id = {id} not found.");

			var result = _brands.DeleteOne(brand => brand.Id == id);
			Log.Warning($"{id} is deleted", result, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<DeleteResult>("", "Delete Brand", $"Brand with id = {id} deletion process attempted.Check result !", result));
		}


	}
}
