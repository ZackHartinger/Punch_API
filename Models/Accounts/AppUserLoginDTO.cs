namespace Punch_API.Models.Accounts
{
    public class AppUserLoginDTO
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool useCookies { get; set; } = true;
    }
}
