using HITSBlazor.Models.Common.Entities;
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
                        MockSkills.GetSkillById(MockSkills.JavaScriptId)!,
                        MockSkills.GetSkillById(MockSkills.CSharpId)!
                    ]
                },
                new IdeaSkills { IdeaId = MockIdeas.Strawberry1Id,  Skills = [.. skillsMocks] }
            ];
        }

        public static List<Skill> GetIdeaSkillsByIdeaId(Guid ideaId)
            => _ideaSkills.FirstOrDefault(isk => isk.IdeaId == ideaId)?.Skills ?? [];

        public static bool CreateOrUpdateIdeasSkills(Guid ideaId, List<Skill> skills)
        {
            var ideasSkills = _ideaSkills.FirstOrDefault(isk => isk.IdeaId == ideaId);
            if (ideasSkills is null)
                _ideaSkills.Add(new IdeaSkills { IdeaId = ideaId, Skills = skills });
            else
                ideasSkills.Skills = skills;

            return true;
        }
    }
}
