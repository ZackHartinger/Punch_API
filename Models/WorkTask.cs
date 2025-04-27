using Punch_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Punch_API.Models;

public partial class WorkTask
{
    public WorkTask() => WorkDayTasks = new HashSet<WorkDayTask>();

    public int WorkTaskId { get; set; }

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    [DefaultValue(false)]
    public bool IsDeprecated { get; set; }

    [JsonIgnore]
    public ICollection<WorkDayTask>? WorkDayTasks { get; set; }

    public int CompanyId { get; set; }

    public Company Company { get; set; }
}
