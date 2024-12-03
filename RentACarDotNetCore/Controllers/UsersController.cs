using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<MongoIdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<MongoIdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // POST api/<UsersController>
        [HttpPost("CreateUser")]
        [Authorize]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
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
                    Name = "admin",
                    NormalizedName = "ADMIN"
                };
                var resultRole = await _roleManager.CreateAsync(role);
                await _userManager.AddToRoleAsync(user, "admin");

                await _signInManager.SignInAsync(user, false);
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }

        [HttpPost("LoginUser")]
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

        //GET: api/<UsersController>
        [Authorize]
        [HttpGet("LogOut")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful.");
        }
    }

}
