using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
using MernisServiceReference;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Application.Responses.User;
using RentACarDotNetCore.Domain.Entities;
using RentACarDotNetCore.Domain.Repositories;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UtilitiesClassLibrary.Exceptions;
using UtilitiesClassLibrary.Helpers;
using Serilog;
using System.Diagnostics;
using System.Data;
using Domain.Entities;
using RentACarDotNetCore.Application.Responses.Brand;
using EmailService.Application.DTOs;
using RabbitMQ.Infrastructure.Abstract;

namespace RentACarDotNetCore.Application.Services
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
		private readonly IPublisher _publisher;
		private readonly string key;
		public UserService(IConfiguration configuration,
			UserManager<User> userManager, RoleManager<MongoIdentityRole> roleManager, SignInManager<User> signInManager,
			IRentACarDatabaseSettings databaseSettings, IMongoClient mongoClient,
			IMapper mapper, IStringConverter stringConverter, IPublisher publisher)
		{
			var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
			_users = database.GetCollection<User>(databaseSettings.UsersCollectionName);
			_JWTUsers = database.GetCollection<JWTUser>(databaseSettings.JWTUsersCollectionName);
			_mapper = mapper;
			_stringConverter = stringConverter;
			_userManager = userManager;  //mongo identity
			_roleManager = roleManager;//mongo identity
			_signInManager = signInManager;//mongo identity
			_publisher = publisher;

			var jwtSettings = configuration.GetSection("Jwt");
			key = jwtSettings["Key"].ToString();
		}
		public async Task<GetUserResponse> Get(string id)
		{
			User existsUser = new User();
			existsUser = _users.Find(user => user.Id.ToString() == id).FirstOrDefault();

			if (existsUser == null)
				throw new NotFoundException($"No user found with this {id}");

			return _mapper.Map<GetUserResponse>(existsUser);
		}

		public async Task<ActionResult<List<GetUserResponse>>> Get()
		{
			List<User> users = _users.Find(user => true).ToList();
			List<GetUserResponse> result = new List<GetUserResponse>();
			GetUserResponse getUserResponse = new GetUserResponse();
			foreach (var user in users)
			{
				getUserResponse = _mapper.Map<GetUserResponse>(user);
				var roles = await _userManager.GetRolesAsync(user);
				getUserResponse.Roles.Clear();
				getUserResponse.Roles.AddRange(roles);
				result.Add(getUserResponse);
			}
			return result;
		}

		public async Task<ActionResult<GetUserResponse>> GetUser(string username)
		{
			User user = _users.Find(user => user.UserName.Equals(username)).FirstOrDefault();
			if (user == null)
				throw new NotFoundException($"{username} not found.");

			var roles = await _userManager.GetRolesAsync(user);
			GetUserResponse response = new GetUserResponse();
			response = _mapper.Map<GetUserResponse>(user);
			response.Roles.Clear();
			response.Roles.AddRange(roles);
			return response;
		}

		public async Task<ActionResult<GetUserResponse>> UpdateRoleMongoUser(UpdateRoleMongoUser updateRoleMongoUser)
		{
			User user = _users.Find(user => user.UserName.Equals(updateRoleMongoUser.UserName)).FirstOrDefault();
			IList<string> roles = new List<string>();

			if (user != null)
			{
				roles = await _userManager.GetRolesAsync(user);
				foreach (var role in roles)
				{
					await _userManager.RemoveFromRoleAsync(user, role);
				}

				foreach (var role in updateRoleMongoUser.Roles.ToLower().Split(","))
				{
					if (!(role == null || role.Equals("")))
					{
						var isExistsRole = await _roleManager.RoleExistsAsync(role.ToLower());
						if (isExistsRole)
						{
							await _userManager.AddToRoleAsync(user, role.ToLower());
						}
						else
						{
							var newRole = new MongoIdentityRole
							{
								Name = role.ToLower(),
								NormalizedName = role.ToUpper()
							};
							var resultRole = await _roleManager.CreateAsync(newRole);
							await _userManager.AddToRoleAsync(user, role.ToLower());
						}
					}

				}
			}
			else
			{
				throw new NotFoundException($"{updateRoleMongoUser.UserName} user was not found.");
			}

			roles = await _userManager.GetRolesAsync(user);
			GetUserResponse response = new GetUserResponse();
			response = _mapper.Map<GetUserResponse>(user);
			response.Roles.Clear();
			response.Roles.AddRange(roles);
			return response;
		}
		public async Task<ActionResult> CreateMongoIdentityUser(CreateUserRequest createUserRequest)
		{
			// TC kimlik kontrolünü kapalı.
			//    if(!CheckUser(createUserRequest.FirstName, createUserRequest.LastName, createUserRequest.NationalIdentity, createUserRequest.DateOfBirthYear))
			//        throw new NotFoundException("Please check your information again. User could not be created.");

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

				//await _signInManager.SignInAsync(user, false);
				var result2 = await _signInManager.PasswordSignInAsync(createUserRequest.UserName, createUserRequest.Password, false, false);
				Debug.WriteLine(result2);
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

		public void Delete(string id)
		{
			User existsUser = new User();
			existsUser = _users.Find(user => user.Id.ToString() == id).FirstOrDefault();

			if (existsUser == null)
				throw new NotFoundException($"No user found with this {id}");

			var result = _users.DeleteOne(user => user.Id.ToString() == id);
			Log.Warning($"{id} is deleted", result, DateTime.UtcNow);
			_publisher.PublishMail(new MailDTO<DeleteResult>("", "Delete User", $"User with id = {id} deletion process attempted.Check result !", result));
		}
	}
}
