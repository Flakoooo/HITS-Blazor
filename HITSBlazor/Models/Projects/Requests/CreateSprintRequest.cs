namespace HITSBlazor.Models.Projects.Requests
{
    public class CreateSprintRequest
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public int WorkingHours { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly FinishDate { get; set; }
        public ICollection<Guid> Tasks { get; set; } = [];
    }
}
