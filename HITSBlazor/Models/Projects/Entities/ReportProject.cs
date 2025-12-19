namespace HITSBlazor.Models.Projects.Entities
{
    public class ReportProject
    {
        public Guid ProjectId { get; set; }
        public List<AverageMark> Marks { get; set; } = [];
        public string Report { get; set; } = string.Empty;
    }
}
