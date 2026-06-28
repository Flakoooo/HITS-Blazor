namespace HITSBlazor.Models.Projects.Requests
{
    public class UpdateSprintRequest
    {
        public string? Name { get; set; }
        public string? Goal { get; set; }
        public int? WorkingHours { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? FinishDate { get; set; }
        public ICollection<Guid>? Tasks { get; set; }
    }
}
