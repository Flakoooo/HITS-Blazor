using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockTaskMovementLogs
    {
        private static readonly Random _random = new();
        private static readonly List<TaskMovementLog> _taskMovementLogs = CreateLogs();

        private static List<TaskMovementLog> CreateLogs()
        {
            var logs = new List<TaskMovementLog>();
            foreach (var sprint in MockSprints.GetAllMockSprints())
            {
                var project = MockProjects.GetProjectById(sprint.ProjectId);
                if (project is null) continue;

                var workablemembers = project.Members.Where(m => m.ProjectRole is not ProjectMemberRole.Initiator).Select(m => m.UserId).ToList();

                var sprintDifference = sprint.FinishDate - sprint.StartDate;

                foreach (var task in sprint.Tasks)
                {
                    if (task.Status is HITSTaskStatus.InBackLog || task.StartDate < sprint.StartDate)
                    {
                        logs.Add(
                            CreateTaskLog(
                                HITSTaskStatus.InBackLog, 
                                task, 
                                null, 
                                task.Initiator.Id, 
                                sprint
                            )
                        );
                    }

                    if (task.Status >= HITSTaskStatus.NewTask)
                    {
                        logs.Add(
                            CreateTaskLog(
                                HITSTaskStatus.NewTask, 
                                task,
                                logs.LastOrDefault(l => l.Task.Id == task.Id), 
                                workablemembers[workablemembers.Count - 1 - _random.Next(0, workablemembers.Count)], 
                                sprint
                            )
                        );
                    }

                    if (task.Status >= HITSTaskStatus.InProgress)
                    {
                        logs.Add(
                            CreateTaskLog(
                                HITSTaskStatus.InProgress, 
                                task,
                                logs.LastOrDefault(l => l.Task.Id == task.Id), 
                                workablemembers[workablemembers.Count - 1 - _random.Next(0, workablemembers.Count)], 
                                sprint
                            )
                        );
                    }

                    if (task.Status >= HITSTaskStatus.OnVerification)
                    {
                        logs.Add(
                            CreateTaskLog(
                                HITSTaskStatus.OnVerification, 
                                task, 
                                logs.LastOrDefault(l => l.Task.Id == task.Id), 
                                workablemembers[workablemembers.Count - 1 - _random.Next(0, workablemembers.Count)], 
                                sprint
                            )
                        );
                    }

                    if (task.Status >= HITSTaskStatus.Done)
                    {
                        logs.Add(
                            CreateTaskLog(
                                HITSTaskStatus.Done, 
                                task, 
                                logs.LastOrDefault(l => l.Task.Id == task.Id), 
                                workablemembers[workablemembers.Count - 1 - _random.Next(0, workablemembers.Count)], 
                                sprint
                            )
                        );
                    }
                }
            }

            return logs;
        }

        private static TaskMovementLog CreateTaskLog(
            HITSTaskStatus targetStatus, HITSTask task, TaskMovementLog? lastLog, Guid executorId, Sprint sprint
        )
        {
            DateTime? startDate = null;

            if (targetStatus is HITSTaskStatus.NewTask)
            {
                startDate = sprint.StartDate;
                lastLog?.EndDate = sprint.StartDate;
            }
            else if (lastLog?.EndDate is not null)
            {
                startDate = lastLog.EndDate.Value;
            }

            var newlog = new TaskMovementLog()
            {
                Id = Guid.NewGuid(),
                Task = task,
                Executor = lastLog is not null ? MockUsers.GetUserById(executorId) : task.Initiator,
                User = task.Initiator,
                StartDate = startDate ?? task.StartDate,
                EndDate = task.Status > targetStatus
                    ? (startDate ?? task.StartDate).AddDays(_random.Next(0, (sprint.FinishDate - (lastLog?.EndDate is null
                        ? sprint.StartDate
                        : lastLog.EndDate.Value)).Days))
                    : null,
                WastedTime = string.Empty,
                Status = targetStatus
            };

            if (targetStatus is HITSTaskStatus.Done)
                task.FinishDate = newlog.EndDate;

            return newlog;
        }

        public static List<TaskMovementLog> GetTaskMovementLogsByTaskId(Guid taskId)
            => [.. _taskMovementLogs.Where(tml => tml.Task.Id == taskId)];

        public static bool CreateNewTaskLog(HITSTask task, User creator, User? executor = null)
        {
            var lastLog = _taskMovementLogs.LastOrDefault(tml => tml.Task.Id == task.Id);
            TaskMovementLog newLog;
            if (lastLog is null)
            {
                newLog = new TaskMovementLog
                {
                    Id = Guid.NewGuid(),
                    Task = task,
                    Executor = executor ?? creator,
                    User = creator,
                    StartDate = DateTime.UtcNow,
                    Status = task.Status
                };
            }
            else
            {
                var endDate = DateTime.UtcNow;

                lastLog.EndDate = endDate;
                lastLog.WastedTime = (lastLog.EndDate - lastLog.StartDate).ToString() ?? string.Empty;

                newLog = new TaskMovementLog
                {
                    Id = Guid.NewGuid(),
                    Task = task,
                    Executor = executor ?? creator,
                    User = creator,
                    StartDate = endDate,
                    Status = task.Status
                };
            }

            _taskMovementLogs.Add(newLog);

            return true;
        }
    }
}
