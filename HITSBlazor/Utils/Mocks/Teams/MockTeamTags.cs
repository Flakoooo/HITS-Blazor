using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamTags
    {
        private static readonly List<TeamTags> _teamTags = CreateTeamTags();

        private static List<TeamTags> CreateTeamTags() => [
            new TeamTags { StudyGroups = ["ИИП-22-1", "ИСТНб-21"],      StudyCourses = [Course.first, Course.second]    },
            new TeamTags { StudyGroups = ["ИИПб-23-1", "АСОИУ-22-1"],   StudyCourses = [Course.second]                  },
            //new TeamTags { StudyGroups = ["ИСТНб-21-2", "АСОИУ-20-1"],  StudyCourses = [Course.third, Course.fourth]    }
        ];

        public static List<TeamTags> GetAllTeamTags() => _teamTags;
    }
}
