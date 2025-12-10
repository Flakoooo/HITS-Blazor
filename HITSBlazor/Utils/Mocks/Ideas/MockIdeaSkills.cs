using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public static class MockIdeaSkills
    {
        private static readonly List<IdeaSkills> _ideaSkills = CreateIdeaSkills();

        private static List<IdeaSkills> CreateIdeaSkills()
        {
            var skillsMocks = MockSkills.GetAllSkills();

            return
            [
                new IdeaSkills { IdeaId = MockIdeas.RefactorId,     Skills = [.. skillsMocks] },
                new IdeaSkills { 
                    IdeaId = MockIdeas.MyNewIdeaId, 
                    Skills =
                    [
                        MockSkills.GetSkillById(MockSkills.JavaScriptId),
                        MockSkills.GetSkillById(MockSkills.CSharpId)
                    ]
                },
                new IdeaSkills { IdeaId = MockIdeas.Strawberry1Id,  Skills = [.. skillsMocks] }
            ];
        }

        public static IdeaSkills? GetIdeaSkillsByIdeaId(string ideaId)
            => _ideaSkills.FirstOrDefault(isk => isk.IdeaId == ideaId);
    }
}
