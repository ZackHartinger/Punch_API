using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Punch_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Punch_API.Models.Users;

public partial class AppUser : IdentityUser<int>
{
    // Default constructor contains navigation properties so that when creating new AppUser objects we can add a company to the
    // user's Companies array. For the time being, only one company will be using this application so a default CompanyId will be added
    // in the controller's method
    public AppUser()
    {
        EmployeeWorkDays = new HashSet<EmployeeWorkDay>();
        Companies = new HashSet<Company>();
    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    // Navigation properties
    [JsonIgnore]
    public ICollection<EmployeeWorkDay>? EmployeeWorkDays { get; set; }

    public ICollection<Company> Companies { get; set; }

}
