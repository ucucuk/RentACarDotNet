using AutoMapper;
using Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RentACarDotNetCore.Application.DTOs;
using RentACarDotNetCore.Application.Requests.Brand;
using RentACarDotNetCore.Application.Responses.Brand;
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
        private readonly IRedisCacheService _redisCacheService;

        public BrandService(IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient, IRedisCacheService redisCacheService, IMapper mapper, IStringConverter stringConverter)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _brands = database.GetCollection<Brand>(databaseSettings.BrandsCollectionName);
            _models = database.GetCollection<Model>(databaseSettings.ModelsCollectionName);
            _stringConverter = stringConverter;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
            watch.Stop();
            Console.WriteLine($"Uygulama Vakti BrandService: {watch.ElapsedMilliseconds} ms");
        }

        public GetBrandResponse Get(string id)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Brand existsBrand = new Brand();
            var cacheData = _redisCacheService.GetOrAdd("allbrands", () => { return _brands.Find(brand => true).ToList(); });
            if (cacheData != null)
            {
                existsBrand = cacheData.FirstOrDefault(b => b.Id == id);
                //existsBrand = cacheData.Where(b => b.Id == id).FirstOrDefault();
            }
            watch.Stop();
            Console.WriteLine($"Uygulama Vakti Cache: {watch.ElapsedMilliseconds} ms");
            if (existsBrand == null)
                throw new NotFoundException($"No brand found with this {id}");

            return _mapper.Map<GetBrandResponse>(existsBrand);

        }

        public List<GetBrandResponse> Get()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            List<GetBrandResponse> list = new List<GetBrandResponse>();
            list = _mapper.Map<List<GetBrandResponse>>(_brands.Find(brand => true).ToList());
            watch.Stop();
            Console.WriteLine($"Uygulama Vakti Database: {watch.ElapsedMilliseconds} ms");
            return list;
        }

        public List<GetBrandResponse> GetCache()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            List<GetBrandResponse> list = new List<GetBrandResponse>();
            list = _redisCacheService.GetOrAdd("allbrands", () => { return _mapper.Map<List<GetBrandResponse>>(_brands.Find(brand => true).ToList()); });
            watch.Stop();
            Console.WriteLine($"Uygulama Vakti Cache: {watch.ElapsedMilliseconds} ms");
            return list;
        }

        public async Task<List<GetBrandWithModelsResponse>> GetBrandWithModels()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            List<Brand> brands = _brands.Find(brand => true).ToList();
            foreach (Brand brand in brands)
            {
                brand.Models = await _models.Find(model => model.Brand.Id == brand.Id).ToListAsync();
            }
            watch.Stop();
            Console.WriteLine($"Uygulama Vakti GetBrandWithModelsResponse Database: {watch.ElapsedMilliseconds} ms");
            return _mapper.Map<List<GetBrandWithModelsResponse>>(brands);
        }

        public async Task<List<GetBrandWithModelsResponse>> GetBrandWithModelsCache()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            List<Brand> brands = await _redisCacheService.GetOrAddAsync("allbrands", async () => await _brands.Find(brand => true).ToListAsync());
            var cacheDataModels = await _redisCacheService.GetOrAddAsync("allmodels", async () => await _models.Find(model => true).ToListAsync());
            foreach (Brand brand in brands)
            {
                brand.Models = cacheDataModels.Where(model => model.Brand.Id == brand.Id).ToList();
            }
            watch.Stop();
            Console.WriteLine($"Uygulama Vakti GetBrandWithModelsResponse Cache: {watch.ElapsedMilliseconds} ms");
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
