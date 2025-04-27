using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Punch_API.Models.Users
{
    public class AppUserStore : UserStore<AppUser, IdentityRole<int>, PunchDbContext, int>
    {
        public AppUserStore(PunchDbContext context) : base(context) { }
    }
}
