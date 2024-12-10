using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Application.Services;
using RentACarDotNetCore.Domain.Entities;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{

    
    [Authorize(AuthenticationSchemes = "Identity.Application,"+ JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return _userService.Get();
        }

        // POST api/<UsersController>
        [AllowAnonymous]
        [HttpPost("CreateMongoIdentityUser")]
        public async Task<ActionResult> CreateMongoIdentityUser([FromBody] CreateUserRequest createUserRequest)
        {
            return await _userService.CreateMongoIdentityUser(createUserRequest);
        }

        [AllowAnonymous]
        [HttpPost("LoginMongoIdentityUser")]
        public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest loginUserRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUserRequest.UserName, loginUserRequest.Password, false, false);
            if (result.Succeeded)
            {
                return Ok(result);
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
            return  _userService.CreateJWTUser(createUserRequest);
        }


        [AllowAnonymous]
        [HttpPost("LoginJWTUser")]
        public ActionResult LoginJWTUser([FromBody] LoginUserRequest loginUserRequest)
        {
            var token = _userService.Authenticate(loginUserRequest.UserName, loginUserRequest.Password);
                if(token == null)
                return Unauthorized();

                return Ok(new { token, loginUserRequest });
        }

        //GET: api/<UsersController>
        [HttpGet("LogOut")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful.");
        }
    }


}
