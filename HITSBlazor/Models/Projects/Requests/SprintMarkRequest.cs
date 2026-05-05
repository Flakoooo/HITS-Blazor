namespace HITSBlazor.Models.Projects.Requests
{
    public class SprintMarkRequest
    {
        public Guid SprintId { get; set; }
        public Guid UserId { get; set; }
        public int Mark { get; set; }
    }
}
