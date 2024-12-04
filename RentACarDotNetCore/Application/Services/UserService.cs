using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Domain.Entities;
using RentACarDotNetCore.Domain.Repositories;
using RentACarDotNetCore.Utilities.Helpers;

namespace RentACarDotNetCore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<MongoIdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMongoCollection<User> _users;
        private readonly IMapper _mapper;
        private readonly IStringConverter _stringConverter;
        public UserService(UserManager<User> userManager, IRentACarDatabaseSettings databaseSettings, 
            IMongoClient mongoClient, IMapper mapper, IStringConverter stringConverter, 
            RoleManager<MongoIdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _users = database.GetCollection<User>(databaseSettings.UsersCollectionName);
            _stringConverter = stringConverter;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public List<User> Get()
        {
            List<User> users = _users.Find(user => true).ToList();
            return users;
        }
        public async Task<ActionResult> Create(CreateUserRequest createUserRequest)
        {
            var user = new User
            {
                UserName = createUserRequest.UserName
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
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
