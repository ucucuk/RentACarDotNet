
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RedisEntegrationBusinessDotNetCore.Concrete;
using RentACarDotNetCore.Application.Services;
using RentACarDotNetCore.Domain.Entities;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Exceptions;
using RentACarDotNetCore.Utilities.Helpers;
using StackExchange.Redis;
using System.Configuration;
using System.Reflection;
using System.Text;


internal class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);



        // Add services to the container.
        /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        //Redis
        var redisConnection = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
        builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

        //Authentication
        {
            // JWT + MONGO IDENTİTY
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"].ToString());

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                option.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
                //ValidateLifetime = true,
                //ValidIssuer = "your-issuer",
                //ValidAudience = "your-audience",
            };
        })
        .AddIdentityCookies(o => { });  //mongo identity

            builder.Services.AddIdentityCore<User>(option =>
            {
                //option.Password.RequireDigit = false;
                //option.User.AllowedUserNameCharacters = new[] { "asd" };
            }
            )
            .AddRoles<MongoIdentityRole>()
            .AddMongoDbStores<User, MongoIdentityRole, Guid>(
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
        }

        {
            //    // Sadece Mongo Identity
            //    builder.Services.AddAuthentication(option =>
            //    {
            //        option.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            //        option.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            //    }
            //     ).AddIdentityCookies(o => { });

            //    builder.Services.AddIdentityCore<User>(option =>
            //    {
            //        //option.Password.RequireDigit = false;
            //        //option.User.AllowedUserNameCharacters = new[] { "asd" };
            //    }
            //    )
            //    .AddRoles<MongoIdentityRole>()
            //    .AddMongoDbStores<User, MongoIdentityRole, Guid>(
            //        builder.Configuration.GetValue<string>("RentACarDatabaseSettings:ConnectionString"),
            //        builder.Configuration.GetValue<string>("RentACarDatabaseSettings:DatabaseName"))
            //    .AddSignInManager()
            //    .AddDefaultTokenProviders();

            //    builder.Services.ConfigureApplicationCookie(option =>
            //    {
            //        option.Cookie.HttpOnly = true;
            //        option.ExpireTimeSpan = TimeSpan.FromMinutes(1);
            //        option.SlidingExpiration = true;
            //        //option.Cookie.Expiration = TimeSpan.FromMinutes(1);
            //        //option.Cookie.MaxAge = TimeSpan.FromMinutes(1);
            //    });
        }

        {
            // Sadece JWT
            // JWT Ayarlarını Okuma
            //var jwtSettings = builder.Configuration.GetSection("Jwt");
            //var key = Encoding.ASCII.GetBytes(jwtSettings["Key"].ToString());
            //builder.Services.AddAuthentication(option =>
            //{
            //    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}
            //)
            //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //    {
            //        options.RequireHttpsMetadata = false;
            //        options.SaveToken = true;
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = false,
            //            ValidateAudience = false,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(key)
            //            //ValidateLifetime = true,
            //            //ValidIssuer = "your-issuer",
            //            //ValidAudience = "your-audience",
            //        };
            //    });
        }


        // MongoDB bağlantı ayarlarını yapılandırma
        builder.Services.Configure<RentACarDatabaseSettings>(
                        builder.Configuration.GetSection(nameof(RentACarDatabaseSettings)));
        // apsettings.json dosyasındaki RentACarDatabaseSettings başlığındaki bilgileri  
        // RentACarDatabaseSettings classındaki değişkenlere atar.


        builder.Services.AddSingleton<IRentACarDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<RentACarDatabaseSettings>>().Value);
        // IRentACarDatabaseSettings interface çağrıldığında RentACarDatabaseSettings classını kullanacağını setliyoruz.


        builder.Services.AddSingleton<IMongoClient>(s =>
                new MongoClient(builder.Configuration.GetValue<string>("RentACarDatabaseSettings:ConnectionString")));
        // Mongodbye databasee nasıl bağlanacağını söylüyoruz.


        // bağımlılıklar
        builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

        builder.Services.AddScoped<IBrandService, BrandService>();
        // IBrandService çağrıldığında BrandService classını kullanacağını söylüyoruz.

        builder.Services.AddScoped<IModelService, ModelService>();
        // IModelService çağrıldığında ModelService classını kullanacağını söylüyoruz.

        builder.Services.AddScoped<ICarService, CarService>();
        // IModelService çağrıldığında ModelService classını kullanacağını söylüyoruz.

        builder.Services.AddScoped<IUserService, UserService>();
        // IUserService çağrıldığında UserService classını kullanacağını söylüyoruz.

        builder.Services.AddScoped<IStringConverter, StringConverter>();
        // IModelService çağrıldığında ModelService classını kullanacağını söylüyoruz.
        //////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        //////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////
        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.UseAuthentication();
        //app.UseRouting();
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

        app.MapControllers();

        app.Run();
    }
}