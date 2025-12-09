using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public static class MockIdeaSkills
    {
        public static List<IdeaSkills> GetMockIdeaSkills()
        {
            var skillsMocks = MockSkills.GetMockSkills();

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
            => GetMockIdeaSkills().FirstOrDefault(isk => isk.IdeaId == ideaId);

        public static List<Skill> GetSkillsForIdea(string ideaId)
            => GetIdeaSkillsByIdeaId(ideaId)?.Skills ?? [];

        public static List<IdeaSkills> GetIdeasWithSkill(string skillId)
            => [.. GetMockIdeaSkills().Where(isk => isk.Skills.Any(s => s.Id == skillId))];

        public static List<string> GetIdeaIdsWithSkill(string skillId)
            => [.. GetIdeasWithSkill(skillId).Select(isk => isk.IdeaId)];

        public static bool IdeaHasSkill(string ideaId, string skillId)
            => GetSkillsForIdea(ideaId).Any(s => s.Id == skillId);

        public static void AddSkillToIdea(string ideaId, Skill skill)
        {
            var ideaSkills = GetIdeaSkillsByIdeaId(ideaId);
            if (ideaSkills != null && !ideaSkills.Skills.Any(s => s.Id == skill.Id))
                ideaSkills.Skills.Add(skill);
        }

        public static void RemoveSkillFromIdea(string ideaId, string skillId)
        {
            var ideaSkills = GetIdeaSkillsByIdeaId(ideaId);
            ideaSkills?.Skills.RemoveAll(s => s.Id == skillId);
        }

        public static List<Skill> GetMissingSkillsForTeam(string ideaId, List<Skill> teamSkills)
        {
            var requiredSkills = GetSkillsForIdea(ideaId);
            var teamSkillIds = teamSkills.Select(s => s.Id).ToList();

            return [.. requiredSkills.Where(required => !teamSkillIds.Contains(required.Id))];
        }
    }
}
