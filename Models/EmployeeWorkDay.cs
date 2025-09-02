using OBD_API.Models;
using Punch_API.Models;
using Punch_API.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

public static class EmployeeWorkDayExtensions
{
    public static EmployeeWorkDayDTO ToDTO(this EmployeeWorkDay employeeWorkDay)
    {
        var workTaskDTOs = new List<WorkTaskDTO>();
        foreach(WorkDayTask workTask in employeeWorkDay.WorkDayTasks)
        {
            workTaskDTOs.Add(new WorkTaskDTO
            {
                WorkTaskId = workTask.WorkTask.WorkTaskId,
                Category = workTask.WorkTask.Category,
                Description = workTask.WorkTask.Description,
                CompanyId = workTask.WorkTask.CompanyId,
                IsDeprecated = workTask.WorkTask.IsDeprecated
            });
        }
        EmployeeWorkDayDTO employeeWorkDayDTO = new EmployeeWorkDayDTO
        {
            EmployeeWorkDayId = employeeWorkDay.EmployeeWorkDayId,
            CustomerName = employeeWorkDay.CustomerName,
            Date = employeeWorkDay.Date,
            StartTime = employeeWorkDay.StartTime,
            EndTime = employeeWorkDay.EndTime,
            LunchTime = employeeWorkDay.LunchTime,
            LunchDuration = employeeWorkDay.LunchDuration,
            Mileage = employeeWorkDay.Mileage,
            TruckName = employeeWorkDay.TruckName,
            UserId = employeeWorkDay.UserId,
            User = new AppUserDTO
            {
                Id = employeeWorkDay.UserId,
                FirstName = employeeWorkDay.User.FirstName,
                LastName = employeeWorkDay.User.LastName,
                FullName = employeeWorkDay.User.FullName,
                Email = employeeWorkDay.User.Email,
                Companies = employeeWorkDay.User.Companies
            },
            WorkDayTasks = workTaskDTOs
        };
        return employeeWorkDayDTO;
    }
}
