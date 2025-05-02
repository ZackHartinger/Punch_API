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
    public AppUser()
    {
        EmployeeWorkDays = new HashSet<EmployeeWorkDay>();
        Companies = new HashSet<Company>();
    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    [JsonIgnore]
    public ICollection<EmployeeWorkDay>? EmployeeWorkDays { get; set; }

    public ICollection<Company> Companies { get; set; }

}
