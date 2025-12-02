using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Tests.Entities
{
    public class TestAnswer
    {
        public string Id { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
        public User User { get; set; } = new();
        public string QuestionName { get; set; } = string.Empty;
        public int QuestionModuleNumber { get; set; }
        public int QuestionNumber { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
