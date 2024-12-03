
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RentACarDotNetCore.Application.Services;
using RentACarDotNetCore.Domain.Entities;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;
using RentACarDotNetCore.Utilities.Helpers;
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


        //Authentication
        builder.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            option.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }
        ).AddIdentityCookies(o=> { });

        builder.Services.AddIdentityCore<User>(option =>
        {
            //option.Password.RequireDigit = false;
            //option.User.AllowedUserNameCharacters = new[] { "asd" };
        }
        )
        .AddRoles<MongoIdentityRole>()
        .AddMongoDbStores<User, MongoIdentityRole, Guid >(
            builder.Configuration.GetValue<string>("RentACarDatabaseSettings:ConnectionString"),
            builder.Configuration.GetValue<string>("RentACarDatabaseSettings:DatabaseName"))
        .AddSignInManager()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(option =>
        {
            option.Cookie.HttpOnly = true;
            option.ExpireTimeSpan = TimeSpan.FromMinutes(1);
            option.SlidingExpiration = true;
            //option.Cookie.Expiration = TimeSpan.FromMinutes(1);
            //option.Cookie.MaxAge = TimeSpan.FromMinutes(1);
        });
        // MongoDB ba�lant� ayarlar�n� yap�land�rma
        builder.Services.Configure<RentACarDatabaseSettings>(
                        builder.Configuration.GetSection(nameof(RentACarDatabaseSettings)));
        // apsettings.json dosyas�ndaki RentACarDatabaseSettings ba�l���ndaki bilgileri
        // RentACarDatabaseSettings class�ndaki de�i�kenlere atar.


        builder.Services.AddSingleton<IRentACarDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<RentACarDatabaseSettings>>().Value);
        // IRentACarDatabaseSettings interface �a�r�ld���nda RentACarDatabaseSettings class�n� kullanaca��n� setliyoruz.


        builder.Services.AddSingleton<IMongoClient>(s =>
                new MongoClient(builder.Configuration.GetValue<string>("RentACarDatabaseSettings:ConnectionString")));
        // Mongodbye databasee nas�l ba�lanaca��n� s�yl�yoruz.


        builder.Services.AddScoped<IBrandService, BrandService>();
        // IBrandService �a�r�ld���nda BrandService class�n� kullanaca��n� s�yl�yoruz.

        builder.Services.AddScoped<IModelService, ModelService>();
        // IModelService �a�r�ld���nda ModelService class�n� kullanaca��n� s�yl�yoruz.

        builder.Services.AddScoped<ICarService, CarService>();
        // IModelService �a�r�ld���nda ModelService class�n� kullanaca��n� s�yl�yoruz.


        builder.Services.AddScoped<IStringConverter, StringConverter>();
        // IModelService �a�r�ld���nda ModelService class�n� kullanaca��n� s�yl�yoruz.
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

        app.UseAuthentication();
        app.UseAuthorization();
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