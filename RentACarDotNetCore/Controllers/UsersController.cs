using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Application.Responses.Brand;
using RentACarDotNetCore.Application.Responses.User;
using RentACarDotNetCore.Application.Services;
using RentACarDotNetCore.Domain.Entities;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{


	[Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly UserManager<User> _userManager; //mongo identity
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<MongoIdentityRole> _roleManager;

		public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<MongoIdentityRole> roleManager, IUserService userService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_userService = userService;
		}
		// GET: api/<UsersController>
		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<List<GetUserResponse>>> Get()
		{
			return await _userService.Get();
		}

		// GET api/<UsersController>/5
		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<ActionResult<GetUserResponse>> Get(string id)
		{
			GetUserResponse user = await _userService.Get(id);
			if (user == null)
			{
				return NotFound($"User with Id = {id} not found.");
			}
			return user;
		}

		// POST api/<UsersController>
		[AllowAnonymous]
		[HttpPost("CreateMongoIdentityUser")]
		public async Task<ActionResult> CreateMongoIdentityUser([FromBody] CreateUserRequest createUserRequest)
		{
			return await _userService.CreateMongoIdentityUser(createUserRequest);
		}

		[AllowAnonymous]
		[HttpPost("UpdateRoles")]
		public async Task<ActionResult<GetUserResponse>> UpdateRoleMongoUser([FromBody] UpdateRoleMongoUser updateRoleMongoUser)
		{
			return await _userService.UpdateRoleMongoUser(updateRoleMongoUser);
		}

		[AllowAnonymous]
		[HttpPost("LoginMongoIdentityUser")]
		public async Task<ActionResult<GetUserResponse>> LoginUser([FromBody] LoginUserRequest loginUserRequest)
		{
			var result = await _signInManager.PasswordSignInAsync(loginUserRequest.UserName, loginUserRequest.Password, false, false);
			if (result.Succeeded)
			{
				loginUserRequest.Password = "****";

				return await _userService.GetUser(loginUserRequest.UserName);
			}
			else
			{
				return BadRequest(result);
			}

		}
		// POST api/<UsersController>
		[AllowAnonymous]
		[HttpPost("CreateJWTUser")]
		public ActionResult<JWTUser> CreateJWTUser([FromBody] CreateUserRequest createUserRequest)
		{
			return _userService.CreateJWTUser(createUserRequest);
		}


		[AllowAnonymous]
		[HttpPost("LoginJWTUser")]
		public ActionResult LoginJWTUser([FromBody] LoginUserRequest loginUserRequest)
		{
			var token = _userService.Authenticate(loginUserRequest.UserName, loginUserRequest.Password);
			if (token == null)
				return Unauthorized();

			return Ok(new { token, loginUserRequest });
		}

		//GET: api/<UsersController>
		[HttpGet("LogOut")]
		[AllowAnonymous]
		public async Task<ActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return Ok("Logout successful.");
		}


		//GET: api/<UsersController>
		[HttpGet("CheckLogin")]
		[AllowAnonymous]
		public async Task<ActionResult> CheckLogin()
		{
			var userName = HttpContext.User.Identity.Name;
			var IsInRole = HttpContext.User.IsInRole("normal");
			var admin = HttpContext.User.IsInRole("admin");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return Ok(new { authenticated = true, userName, IsInRole, admin });
			}
			else
			{
				return Ok(new { authenticated = false });
			}
		}


		// DELETE api/<UsersController>/5
		[HttpDelete("{id}")]
		[AllowAnonymous]
		public ActionResult Delete(string id)
		{
			var existingUser = _userService.Get(id);

			if (existingUser == null)
			{
				return NotFound($"User with id = {id} not found.");
			}

			_userService.Delete(id);

			return Ok($"Brand with id = {id} deleted.");
		}
	}


}
