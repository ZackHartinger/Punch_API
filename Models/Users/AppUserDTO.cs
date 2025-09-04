namespace Punch_API.Models.Users
{
    public class AppUserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string FullName { get; set; } = null!;
                    
        public string Email { get; set; } = null!;
        
        public ICollection<Company> Companies { get; set; } = new List<Company>();

        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
