using Punch_API.Models;

namespace OBD_API.Models
{
    public class EmployeeWorkDayDTO
    {
        public EmployeeWorkDayDTO() => WorkDayTasks = new HashSet<WorkTaskDTO>();

        public int EmployeeWorkDayId { get; set; }

        public string CustomerName { get; set; } = null!;

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public TimeOnly LunchTime { get; set; }

        public int LunchDuration { get; set; }

        public int? Mileage { get; set; }

        public string? TruckName { get; set; }

        public int UserId { get; set; }

        public ICollection<WorkTaskDTO> WorkDayTasks { get; set; } 
    }
}
