using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RentACarDotNetCore.Application.Services;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;
using RentACarDotNetCore.Utilities.StringMethods;
using System.Reflection;
internal class Startup
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
      

        builder.Services.Configure<RentACarDatabaseSettings>(
                        builder.Configuration.GetSection(nameof(RentACarDatabaseSettings)));
        // apsettings.json dosyasýndaki RentACarDatabaseSettings baþlýðýndaki bilgileri
        // RentACarDatabaseSettings classýndaki deðiþkenlere atar.


        builder.Services.AddSingleton<IRentACarDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<RentACarDatabaseSettings>>().Value);
        // IRentACarDatabaseSettings interface çaðrýldýðýnda RentACarDatabaseSettings classýný kullanacaðýný setliyoruz.


        builder.Services.AddSingleton<IMongoClient>(s =>
                new MongoClient(builder.Configuration.GetValue<string>("RentACarDatabaseSettings:ConnectionString")));
        // Mongodbye databasee nasýl baðlanacaðýný söylüyoruz.


        builder.Services.AddScoped<IBrandService, BrandService>();
        // IBrandService çaðrýldýðýnda BrandService classýný kullanacaðýný söylüyoruz.

        builder.Services.AddScoped<IModelService, ModelService>();
        // IModelService çaðrýldýðýnda ModelService classýný kullanacaðýný söylüyoruz.

        builder.Services.AddScoped<IStringConverter, StringConverter>();
        // IModelService çaðrýldýðýnda ModelService classýný kullanacaðýný söylüyoruz.
        //////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        //////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////
        app.UseMiddleware<ErrorHandlerMiddleware>();
        //////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}