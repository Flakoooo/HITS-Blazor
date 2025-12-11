using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockSprintMarks
    {
        private static readonly List<SprintMarks> _sprintMarks = CreateSprintMarks();

        private static List<SprintMarks> CreateSprintMarks()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;

            return
            [
                new SprintMarks
                {
                    SprintId = MockSprints.Sprint3Id,
                    UserId = kirill.Id,
                    FirstName = kirill.FirstName,
                    LastName = kirill.LastName,
                    ProjectRole = ProjectMemberRole.TEAM_LEADER,
                    Mark = null,
                    CountCompletedTasks = 2
                },
                new SprintMarks
                {
                    SprintId = MockSprints.Sprint3Id,
                    UserId = ivan.Id,
                    FirstName = ivan.FirstName,
                    LastName = ivan.LastName,
                    ProjectRole = ProjectMemberRole.MEMBER,
                    Mark = null,
                    CountCompletedTasks = 2
                },
                new SprintMarks
                {
                    SprintId = MockSprints.Sprint3Id,
                    UserId = manager.Id,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    ProjectRole = ProjectMemberRole.MEMBER,
                    Mark = null,
                    CountCompletedTasks = 2
                }
            ];
        }
    }
}
