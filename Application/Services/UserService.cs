using Application.Requests.User;
using Application.Responses.User;
using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RentACarDotNetCore.Domain.Entities;
using RentACarDotNetCore.Domain.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<MongoIdentityRole> _roleManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IMongoCollection<User> _users;
		private readonly IMongoCollection<JWTUser> _JWTUsers;
		private readonly IMapper _mapper;
		private readonly IStringConverter _stringConverter;
		private readonly string key;
		public UserService(IConfiguration configuration,
			UserManager<User> userManager, RoleManager<MongoIdentityRole> roleManager, SignInManager<User> signInManager,
			IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient,
			IMapper mapper, IStringConverter stringConverter
			)
		{
			var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
			_users = database.GetCollection<User>(databaseSettings.UsersCollectionName);
			_JWTUsers = database.GetCollection<JWTUser>(databaseSettings.JWTUsersCollectionName);
			_mapper = mapper;
			_stringConverter = stringConverter;
			_userManager = userManager;  //mongo identity
			_roleManager = roleManager;//mongo identity
			_signInManager = signInManager;//mongo identity

			var jwtSettings = configuration.GetSection("Jwt");
			key = jwtSettings["Key"].ToString();
		}
		public List<GetUserResponse> Get()
		{
			List<User> users = _users.Find(user => true).ToList();

			return _mapper.Map<List<GetUserResponse>>(users);
		}
		public async Task<ActionResult> CreateMongoIdentityUser(CreateUserRequest createUserRequest)
		{
			if (!CheckUser(createUserRequest.FirstName, createUserRequest.LastName, createUserRequest.NationalIdentity, createUserRequest.DateOfBirthYear))
				throw new NotFoundException("Please check your information again. User could not be created.");

			User existUser = await _userManager.FindByNameAsync(createUserRequest.UserName);
			if (existUser != null)
				throw new AlreadyExistsException($"{createUserRequest.UserName} username already exists.");

			var user = new User
			{
				UserName = createUserRequest.UserName,
				FirstName = createUserRequest.FirstName,
				LastName = createUserRequest.LastName,
				NationalIdentity = createUserRequest.NationalIdentity,
				DateOfBirthYear = createUserRequest.DateOfBirthYear
			};

			var result = await _userManager.CreateAsync(user, createUserRequest.Password);

			if (result.Succeeded)
			{
				var role = new MongoIdentityRole
				{
					Name = "normal",
					NormalizedName = "NORMAL"
				};
				var resultRole = await _roleManager.CreateAsync(role);
				await _userManager.AddToRoleAsync(user, "normal");

				await _signInManager.SignInAsync(user, false);
				return new CreatedResult("User created successfully", user);
			}
			else
			{
				return new UnprocessableEntityResult();
			}
		}

		public JWTUser CreateJWTUser(CreateUserRequest createUserRequest)
		{
			if (!CheckUser(createUserRequest.FirstName, createUserRequest.LastName, createUserRequest.NationalIdentity, createUserRequest.DateOfBirthYear))
				throw new NotFoundException("Please check your information again. User could not be created.");

			JWTUser existjwtUser = _JWTUsers.Find(jwtuser => jwtuser.UserName == createUserRequest.UserName).FirstOrDefault();
			if (existjwtUser != null)
				throw new AlreadyExistsException($"{createUserRequest.UserName} username already exists.");

			JWTUser JWTUser = new JWTUser
			{
				UserName = createUserRequest.UserName,
				Password = createUserRequest.Password,
				FirstName = createUserRequest.FirstName,
				LastName = createUserRequest.LastName,
				NationalIdentity = createUserRequest.NationalIdentity,
				DateOfBirthYear = createUserRequest.DateOfBirthYear
			};

			_JWTUsers.InsertOne(JWTUser);
			return JWTUser;
		}

		public string Authenticate(string username, string password)
		{
			var user = _JWTUsers.Find(jwtuser => jwtuser.UserName.Equals(username) && jwtuser.Password.Equals(password)).FirstOrDefault();
			if (user == null)
				return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.ASCII.GetBytes(key);
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(new Claim[] {
					new Claim(ClaimTypes.Name, username)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(tokenKey),
					SecurityAlgorithms.HmacSha256Signature
					)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public bool CheckUser(string FirstName, string LastName, string NationalIdentity, int DateOfBirthYear)
		{
			KPSPublicSoapClient client = new KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);

			return client.TCKimlikNoDogrulaAsync(new TCKimlikNoDogrulaRequest
				(new TCKimlikNoDogrulaRequestBody
				(Convert.ToInt64(NationalIdentity), FirstName, LastName, DateOfBirthYear)))
				.Result.Body.TCKimlikNoDogrulaResult;
		}
	}


}
