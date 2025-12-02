namespace HITSBlazor.Models.Projects.Entities
{
    public class ReportProject
    {
        public string ProjectId { get; set; } = string.Empty;
        public List<AverageMark> Marks { get; set; } = new();
        public string Report { get; set; } = string.Empty;
    }
}
