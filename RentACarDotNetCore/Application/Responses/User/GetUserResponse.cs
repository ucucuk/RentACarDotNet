namespace RentACarDotNetCore.Application.Responses.User
{
    public class GetUserResponse
    {
        public DateTime CreatedDate { get; set; }

        public short Status { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        private string nationalidentity;
        public string NationalIdentity {
            get
            {
                return nationalidentity.Substring(0,5)+"******";
            }
            set
            {
                nationalidentity = value;
            }
        }
        public int DateOfBirthYear { get; set; }
    }
}
