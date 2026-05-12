using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockSprintMarks
    {
        private static readonly Random _random = new();

        private static readonly List<SprintMarks> _sprintMarks = CreateSprintMarks();

        private static List<SprintMarks> CreateSprintMarks()
        {
            var sprintMarks = new List<SprintMarks>();

            foreach (var project in MockProjects.GetAllMockProject())
                foreach (var sprint in MockSprints.GetAllMockSprints().Where(s => s.ProjectId == project.Id && s.Status is SprintStatus.Done))
                    foreach (var member in project.Members)
                        sprintMarks.Add(new SprintMarks
                        {
                            SprintId = sprint.Id,
                            UserId = member.UserId,
                            FirstName = member.FirstName,
                            LastName = member.LastName,
                            ProjectRole = member.ProjectRole,
                            Mark = _random.Next(11),
                            CountCompletedTasks = MockSprints.GetMockTasks()
                                .Count(t => t.SprintId == sprint.Id && t.Status is HITSTaskStatus.Done && t.Executor?.Id == member.UserId)
                        });

            return sprintMarks;
        }

        public static List<SprintMarks> GetSprintMarks(Guid projectId, Guid userId)
        {
            var sprintMarks = new List<SprintMarks>();

            foreach (var sprint in MockSprints.GetAllMockSprints().Where(s => s.ProjectId == projectId && s.Status is SprintStatus.Done))
            {
                var sprintMark = _sprintMarks.FirstOrDefault(sm => sm.SprintId == sprint.Id && sm.UserId == userId);
                if (sprintMark is null) continue;

                sprintMarks.Add(sprintMark);
            }

            return sprintMarks;
        }

        public static List<SprintMarks> GetSprintMarksBySprintId(Guid sprintId)
            => _sprintMarks.Where(sm => sm.SprintId == sprintId).ToList();

        public static bool CreateSprintMarks(
            Guid projectId, Guid sprintId, Dictionary<Guid, List<HITSTask>> memberTasks, IEnumerable<SprintMarkRequest> marks
        )
        {
            var marksByUser = marks.ToDictionary(m => m.UserId, m => m.Mark);
            foreach (var tasks in memberTasks)
            {
                var member = MockProjects.GetCurrentProjectMember(projectId, tasks.Key);
                if (member is null) continue;

                var sprintMark = new SprintMarks
                {
                    SprintId = sprintId,
                    UserId = member.UserId,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    ProjectRole = member.ProjectRole,
                    Mark = marksByUser.GetValueOrDefault(member.UserId),
                    CountCompletedTasks = tasks.Value.Count
                };

                _sprintMarks.Add(sprintMark);
            }

            return true;
        }

        public static bool DeleteMarkByMemberId(Guid projectId, Guid memberId)
        {
            foreach (var sprint in MockSprints.GetAllMockSprints(projectId))
            {
                var mark = _sprintMarks.FirstOrDefault(sm => sm.UserId == memberId && sm.SprintId == sprint.Id);
                if (mark is null) continue;
                _sprintMarks.Remove(mark);
            }
            return true;
        }
    }
}
