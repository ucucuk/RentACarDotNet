namespace RentACarDotNetCore.Domain.Repositories
{
    public interface IRentACarDatabaseSettings
    {
        string BrandsCollectionName { get; set; }
        string ModelsCollectionName { get; set; }
        string CarsCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string JWTUsersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }

    }
}
