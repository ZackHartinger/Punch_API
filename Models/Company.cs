using Punch_API.Models;
using Punch_API.Models.Users;
using System.Text.Json.Serialization;

namespace Punch_API.Models
{
    public class Company
    {
        public Company() 
        {
            AppUsers = new HashSet<AppUser>();
            EmployeeWorkDays = new HashSet<EmployeeWorkDay>();
            WorkTasks = new HashSet<WorkTask>();
        }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        // Navigation properties
        [JsonIgnore]
        public ICollection<AppUser> AppUsers { get; set; }

        [JsonIgnore]
        public ICollection<EmployeeWorkDay> EmployeeWorkDays { get; } = new List<EmployeeWorkDay>();

        [JsonIgnore]
        public ICollection<WorkTask> WorkTasks { get; } = new List<WorkTask>();
    }
}
