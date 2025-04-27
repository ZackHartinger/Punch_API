using Punch_API.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Punch_API.Models;

public partial class WorkDayTask
{
    public int WorkDayTaskId { get; set; }

    public int? EmployeeWorkDayId { get; set; }

    [JsonIgnore]
    public EmployeeWorkDay? EmployeeWorkDay { get; set; }

    public int? WorkTaskId { get; set; }

    public WorkTask WorkTask { get; set; } = null!;
}
