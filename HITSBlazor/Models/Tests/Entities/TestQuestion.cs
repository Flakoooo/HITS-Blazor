namespace HITSBlazor.Models.Tests.Entities
{
    public class TestQuestion
    {
        public Guid Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int QuestionNumber { get; set; }
        public string QuestionName { get; set; } = string.Empty;
        public int QuestionModuleNumber { get; set; }
        public string QuestionModule { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}
