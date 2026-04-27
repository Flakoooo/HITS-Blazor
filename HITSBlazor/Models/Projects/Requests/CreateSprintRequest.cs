namespace HITSBlazor.Models.Projects.Requests
{
    public class CreateSprintRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int WorkingHours { get; set; }
        public ICollection<Entities.Task> Tasks { get; set; } = [];
    }
}
