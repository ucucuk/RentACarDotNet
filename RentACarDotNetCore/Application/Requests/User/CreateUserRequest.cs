namespace RentACarDotNetCore.Application.Requests.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalIdentity { get; set; }
        public int DateOfBirthYear { get; set; }

    }
}
