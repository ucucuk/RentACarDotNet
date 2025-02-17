using Microsoft.AspNetCore.Mvc;
using RentACarDotNetCore.Application.Requests.User;
using RentACarDotNetCore.Application.Responses.User;
using RentACarDotNetCore.Domain.Entities;

namespace RentACarDotNetCore.Application.Services
{
	public interface IUserService
	{
		List<GetUserResponse> Get();
		Task<ActionResult<GetUserResponse>> AddRoleMongoUser(AddRoleRequest addRoleRequest);
		Task<ActionResult<GetUserResponse>> GetUser(string username);
		Task<ActionResult> CreateMongoIdentityUser(CreateUserRequest createUserRequest);
		JWTUser CreateJWTUser(CreateUserRequest createUserRequest);
		string Authenticate(string username, string password);
		bool CheckUser(string FirstName, string LastName, string NationalIdentity, int DateOfBirthYear);
	}
}
