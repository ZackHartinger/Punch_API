using Punch_API.Models.Users;

namespace Punch_API.Models
{
    public class AppUserCompany
    {
        public int AppUsersId { get; set; }

        public int CompaniesCompanyId { get; set; }

        public virtual AppUser AppUserRelation { get; set; }

        public virtual Company CompanyRelation { get; set; }
    }
}
