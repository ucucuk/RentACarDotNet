namespace RentACarDotNetCore.Domain.Repositories
{
    public class RentACarDatabaseSettings : IRentACarDatabaseSettings
    {
        public string BrandsCollectionName { get; set; } = string.Empty;
        public string ModelsCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CarsCollectionName { get ; set; } = string.Empty;
    }
}
