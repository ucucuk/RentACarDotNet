using Application.DTOs;
using Application.Requests.Brand;
using Application.Responses.Brand;
using AutoMapper;
using Domain.Entities;
using EmailService.Application.Abstarct;
using EmailService.Application.DTOs;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RabbitMQ.Infrastructure.Abstract;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RentACarDotNetCore.Domain.Repositories;

namespace Application.Services
{
	public class BrandService : IBrandService
	{
		private readonly IMongoCollection<Brand> _brands;
		private readonly IMongoCollection<Model> _models;
		private readonly IMapper _mapper;
		private readonly IStringConverter _stringConverter;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IPublisher _publisher;
		private readonly IConsumer _consumer;


		public BrandService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IRedisCacheService redisCacheService,
			IMapper mapper, IStringConverter stringConverter, IPublisher publisher, IConsumer consumer, IMailService mailService)
		{
			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
			_brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
			_models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
			_stringConverter = stringConverter;
			_mapper = mapper;
			_redisCacheService = redisCacheService;
			_publisher = publisher;
			watch.Stop();
			Console.WriteLine($"Uygulama Vakti BrandService: {watch.ElapsedMilliseconds} ms");
			_consumer = consumer;
		}

		public async Task<GetBrandResponse> Get(string id)
		{
			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
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

			watch.Stop();
			Console.WriteLine($"Uygulama Vakti Cache: {watch.ElapsedMilliseconds} ms");
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

		public async void SendMailFromRabbitMQ()
		{
			await _consumer.ConsumeMailFromRabbitMQ();
		}

		public async Task<List<GetBrandWithModelsResponse>> GetBrandWithModels()
		{
			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			var cacheDataBrands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
			var cacheDataModels = await _redisCacheService.GetOrAddAsync("allmodels", async () => await _models.Find(model => true).ToListAsync());
			foreach (Brand brand in cacheDataBrands)
			{
				brand.Models = cacheDataModels.Where(model => model.Brand.Id == brand.Id).ToList();
			}
			watch.Stop();
			Console.WriteLine($"Uygulama Vakti GetBrandWithModelsResponse Cache: {watch.ElapsedMilliseconds} ms");
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

			_publisher.PublishMail(new MailDTO<BrandDTO>("", "", _mapper.Map<BrandDTO>(brand)));
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
