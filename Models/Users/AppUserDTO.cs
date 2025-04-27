namespace Punch_API.Models.Users
{
    public class AppUserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
