using Punch_API.Models.Users;
using Punch_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Punch_API.Models;

public partial class EmployeeWorkDay
{
    public EmployeeWorkDay() => WorkDayTasks = new HashSet<WorkDayTask>();

    public int EmployeeWorkDayId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DefaultValue(true)]
    public bool IsNew { get; set; } = true;

    public string CustomerName { get; set; } = null!;

    public DateOnly Date { get; set; } 

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public TimeOnly LunchTime { get; set; }

    public int LunchDuration { get; set; }

    public int? Mileage { get; set; }

    public string? TruckName { get; set; }

    public int UserId { get; set; }

    public AppUser User { get; set; } = null!;

    public ICollection<WorkDayTask> WorkDayTasks { get; set; }

    public int CompanyId { get; set; }

    public Company Company { get; set; }
}
