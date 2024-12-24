using Application.Requests.User;
using Application.Responses.User;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Domain.Entities;

namespace Application.Services
{
	public interface IUserService
	{
		List<GetUserResponse> Get();
		Task<ActionResult> CreateMongoIdentityUser(CreateUserRequest createUserRequest);
		JWTUser CreateJWTUser(CreateUserRequest createUserRequest);
		string Authenticate(string username, string password);
		bool CheckUser(string FirstName, string LastName, string NationalIdentity, int DateOfBirthYear);
	}
}
