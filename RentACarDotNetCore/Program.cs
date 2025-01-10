
using AspNetCore.Identity.MongoDbCore.Models;
using EmailService.Application.Abstarct;
using EmailService.Application.Concrete;
using EmailService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RabbitMQ.Application.Abstract;
using RabbitMQ.Application.Concrete;
using RabbitMQ.Infrastructure.Abstract;
using RabbitMQ.Infrastructure.Concrete;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RedisEntegrationBusinessDotNetCore.Concrete;
using RentACarDotNetCore.Application.Services;
using RentACarDotNetCore.Domain.Entities;
using RentACarDotNetCore.Domain.Repositories;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using UtilitiesClassLibrary.Exceptions;
using UtilitiesClassLibrary.Helpers;


internal class Program
{
	private static void Main(string[] args)
	{
		ConfigureLogging();


		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		/////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());


		builder.Host.UseSerilog();

		//CORS AYARI
		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowLocalhost", builder =>
			{
				builder.WithOrigins("https://localhost:44321")
					   .AllowAnyHeader()
					   .AllowAnyMethod();
			});
		});

		//Redis
		var redisConnection = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
		builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection); ;

		//Authentication
		{
			// JWT + MONGO IDENT�TY
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
			// JWT Ayarlar�n� Okuma
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


		// MongoDB ba�lant� ayarlar�n� yap�land�rma
		builder.Services.Configure<RentACarDatabaseSettings>(
						builder.Configuration.GetSection(nameof(RentACarDatabaseSettings)));
		// apsettings.json dosyas�ndaki RentACarDatabaseSettings ba�l���ndaki bilgileri  
		// RentACarDatabaseSettings class�ndaki de�i�kenlere atar.

		builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

		builder.Services.AddSingleton<IRentACarDatabaseSettings>(sp =>
			sp.GetRequiredService<IOptions<RentACarDatabaseSettings>>().Value);
		// IRentACarDatabaseSettings interface �a�r�ld���nda RentACarDatabaseSettings class�n� kullanaca��n� setliyoruz.


		builder.Services.AddSingleton<IMongoClient>(s =>
				new MongoClient(builder.Configuration.GetValue<string>("RentACarDatabaseSettings:ConnectionString")));
		// Mongodbye databasee nas�l ba�lanaca��n� s�yl�yoruz.


		// ba��ml�l�klar
		builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
		builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
		builder.Services.AddScoped<IMailService, MailService>();
		builder.Services.AddScoped<IPublisher, Publisher>();
		builder.Services.AddScoped<IConsumer, Consumer>();

		builder.Services.AddScoped<IBrandService, BrandService>();
		// IBrandService �a�r�ld���nda BrandService class�n� kullanaca��n� s�yl�yoruz.

		builder.Services.AddScoped<IModelService, ModelService>();
		// IModelService �a�r�ld���nda ModelService class�n� kullanaca��n� s�yl�yoruz.

		builder.Services.AddScoped<ICarService, CarService>();
		// IModelService �a�r�ld���nda ModelService class�n� kullanaca��n� s�yl�yoruz.

		builder.Services.AddScoped<IUserService, UserService>();
		// IUserService �a�r�ld���nda UserService class�n� kullanaca��n� s�yl�yoruz.

		builder.Services.AddScoped<IStringConverter, StringConverter>();
		// IModelService �a�r�ld���nda ModelService class�n� kullanaca��n� s�yl�yoruz.
		//////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		builder.Host.UseSerilog();


		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		//////////////////////////////////////////////////////////////////////
		var app = builder.Build();

		//app.UseSerilogRequestLogging();

		app.UseCors("AllowLocalhost"); // CORS politikas�n� kullanma
		app.UseMiddleware<ErrorHandlerMiddleware>();

		app.UseAuthentication();
		//app.UseRouting();
		app.UseAuthorization();
		//////////////////////////////////////////////////////////////////////
		

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.MapControllers();

		try
		{
			app.Run();
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Startup Error!");
		}

	}

	private static void ConfigureLogging()
	{
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
			.Build();

		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.MinimumLevel.Debug()
			.Filter.ByExcluding(Matching.FromSource("Microsoft"))
			//.Filter.ByExcluding(Matching.FromSource("System"))
			.WriteTo.Debug()
			.WriteTo.Console()
			.WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
			.Enrich.WithProperty("Environment", environment)
			.ReadFrom.Configuration(configuration)
			.CreateLogger();
	}

	private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
	{
		return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
		{
			AutoRegisterTemplate = true,
			IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower()
			.Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
			FailureSink = new FileSink("./fail.txt", new JsonFormatter(), null, null)
			//IndexFormat = $"rentacar-{DateTime.UtcNow:yyyy-MM}",

		};
	}

}