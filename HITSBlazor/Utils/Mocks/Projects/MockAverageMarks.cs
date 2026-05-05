using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockAverageMarks
    {
        private static readonly List<AverageMark> _averageMarks = CreateAverageMarks();

        private static List<AverageMark> CreateAverageMarks()
        {
            var averageMarks = new List<AverageMark>();

            foreach (var project in MockProjects.GetAllMockProject())
            {
                foreach (var member in project.Members)
                {
                    var sprintMarks = MockSprintMarks.GetSprintMarks(project.Id, member.UserId);
                    int sumMark = sprintMarks.Sum(sm => sm.Mark) ?? 0;
                    double averageMark = sumMark > 0 ? (double)sumMark / sprintMarks.Count : 0.0;

                    var activeSprint = MockSprints.GetActiveSprintByProjectId(project.Id);
                    var executedTasks = MockSprints.GetMockTasks()
                        .Where(t => t.SprintId != activeSprint?.Id && t.Executor?.Id == member.UserId && t.Status is HITSTaskStatus.Done);

                    var mark = averageMarks.FirstOrDefault(am => am.UserId == member.UserId && am.ProjectId == project.Id);
                    if (mark is null)
                    {
                        averageMarks.Add(new AverageMark
                        {
                            ProjectId = project.Id,
                            UserId = member.UserId,
                            FirstName = member.FirstName,
                            LastName = member.LastName,
                            ProjectRole = ProjectMemberRole.TeamLeader,
                            Mark = averageMark,
                            Tasks = executedTasks.ToList()
                        });
                    }
                }
            }

            return averageMarks;
        }

        public static List<AverageMark> GetAverageMarkByProjectId(Guid projectId)
            => [.. _averageMarks.Where(am => am.ProjectId == projectId)];


        public static bool UpdateProjectMarks(Guid projectId, Dictionary<Guid, List<HITSTask>> memberTasks)
        {
            foreach (var pair in memberTasks)
            {
                var sprintMarks = MockSprintMarks.GetSprintMarks(projectId, pair.Key);
                if (sprintMarks.Count == 0) continue;

                var projectMember = MockProjects.GetCurrentProjectMember(projectId, pair.Key);
                if (projectMember is null) continue;

                int sumMark = sprintMarks.Sum(sm => sm.Mark) ?? 0;
                double averageMark = sumMark > 0 ? (double)sumMark / sprintMarks.Count : 0.0;

                var projectMark = _averageMarks.FirstOrDefault(am => am.ProjectId == projectId && am.UserId == pair.Key);
                if (projectMark is null)
                {
                    var newProjectMark = new AverageMark
                    {
                        ProjectId = projectId,
                        UserId = projectMember.UserId,
                        FirstName = projectMember.FirstName, 
                        LastName = projectMember.LastName,
                        ProjectRole = projectMember.ProjectRole,
                        Mark = averageMark,
                        Tasks = pair.Value.ToList()
                    };
                    _averageMarks.Add(newProjectMark);
                }
                else
                {
                    projectMark.Mark = averageMark;
                    projectMark.Tasks = projectMark.Tasks?
                        .Concat(pair.Value)
                        .DistinctBy(t => t.Id)
                        .ToList();
                }
            }

            return true;
        }
    }
}
