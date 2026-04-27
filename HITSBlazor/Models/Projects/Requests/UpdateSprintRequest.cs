namespace HITSBlazor.Models.Projects.Requests
{
    public class UpdateSprintRequest
    {
        public string? Name { get; set; }
        public string? Goal { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int? WorkingHours { get; set; }
        public ICollection<Entities.Task>? Tasks { get; set; }
    }
}
