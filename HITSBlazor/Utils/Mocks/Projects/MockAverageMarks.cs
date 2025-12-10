using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockAverageMarks
    {
        private static readonly List<AverageMark> _averageMarks = CreateAverageMarks();

        private static List<AverageMark> CreateAverageMarks()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;

            var authIntegrationTask = MockTasks.GetTaskById(MockTasks.AuthIntegrationId);
            var blackThemeTask = MockTasks.GetTaskById(MockTasks.BlackThemeId);
            var fileUploadTask = MockTasks.GetTaskById(MockTasks.FileUploadId);
            var templatesTask = MockTasks.GetTaskById(MockTasks.TaskTemplatesId);

            return
            [
                new AverageMark
                {
                    ProjectId = MockProjects.ChatBotId,
                    UserId = kirill.Id,
                    FirstName = kirill.FirstName,
                    LastName = kirill.LastName,
                    ProjectRole = ProjectMemberRole.TEAM_LEADER,
                    Mark = 9.9,
                    Tasks = [authIntegrationTask, templatesTask]
                },
                new AverageMark
                {
                    ProjectId = MockProjects.ChatBotId,
                    UserId = ivan.Id,
                    FirstName = ivan.FirstName,
                    LastName = ivan.LastName,
                    ProjectRole = ProjectMemberRole.MEMBER,
                    Mark = 6.7,
                    Tasks = [blackThemeTask]
                },
                new AverageMark
                {
                    ProjectId = MockProjects.ChatBotId,
                    UserId = manager.Id,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    ProjectRole = ProjectMemberRole.MEMBER,
                    Mark = 7.8,
                    Tasks = [fileUploadTask]
                }
            ];
        }

        public static List<AverageMark> GetAverageMarkByProjectId(string projectId)
            => [.. _averageMarks.Where(am => am.ProjectId == projectId)];
    }
}
