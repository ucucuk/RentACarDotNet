using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Domain.Entities;

namespace RentACarDotNetCore.Application.Services
{
    public interface IUserService
    {
        List<User> Get();
        Task<ActionResult> CreateMongoIdentityUser(CreateUserRequest createUserRequest);
        JWTUser CreateJWTUser(CreateUserRequest createUserRequest);

        string Authenticate(string username, string password);
    }
}
