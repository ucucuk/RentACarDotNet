using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Domain.Entities;

namespace RentACarDotNetCore.Application.Services
{
    public interface IUserService
    {
        List<User> Get();
        Task<ActionResult> Create(CreateUserRequest createUserRequest);
    }
}
