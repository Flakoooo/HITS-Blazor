using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamTags
    {
        public static List<TeamTags> GetMockTeamTags() => [
            new TeamTags { StudyGroups = ["ИИП-22-1", "ИСТНб-21"],      StudyCourses = [Course.first, Course.second]    },
            new TeamTags { StudyGroups = ["ИИПб-23-1", "АСОИУ-22-1"],   StudyCourses = [Course.second]                  },
            new TeamTags { StudyGroups = ["ИСТНб-21-2", "АСОИУ-20-1"],  StudyCourses = [Course.third, Course.fourth]    }
        ];

        public static List<TeamTags> GetTeamTagsByCourse(Course course)
            => [.. GetMockTeamTags().Where(tt => tt.StudyCourses.Contains(course))];

        public static List<TeamTags> GetTeamTagsByStudyGroup(string studyGroup)
            => [.. GetMockTeamTags().Where(tt => tt.StudyGroups.Contains(studyGroup))];

        public static List<string> GetAllUniqueStudyGroups()
            => [.. GetMockTeamTags().SelectMany(tt => tt.StudyGroups).Distinct()];

        public static List<Course> GetAllUniqueCourses()
            => [.. GetMockTeamTags().SelectMany(tt => tt.StudyCourses).Distinct()];
    }
}
