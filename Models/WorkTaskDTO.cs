namespace OBD_API.Models
{
    public class WorkTaskDTO
    {
        public int WorkTaskId { get; set; }

        public string Category { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}
