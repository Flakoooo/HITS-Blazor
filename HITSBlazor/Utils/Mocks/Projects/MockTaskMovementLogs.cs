using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;
using System.Globalization;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockTaskMovementLogs
    {
        private static readonly Random _random = new();
        private static readonly List<TaskMovementLog> _taskMovementLogs = CreateTaskMovementLogs();

        public static List<TaskMovementLog> CreateTaskMovementLogs()
        {
            var authIntegrationTask = MockSprints.GetTaskById(MockSprints.AuthIntegrationTaskId)!;
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;

            var logs = new List<TaskMovementLog>();

            var newTaskEnd = AddWorkingDays(authIntegrationTask.StartDate, _random.Next(1, 3), 4);
            logs.Add(CreateLog(authIntegrationTask, null, kirill, authIntegrationTask.StartDate, newTaskEnd, HITSTaskStatus.NewTask, 4.0));

            DateTime verificationStart = (DateTime)(newTaskEnd + ((authIntegrationTask.FinishDate! - newTaskEnd) * 0.7));

            logs.AddRange([
                CreateLog(authIntegrationTask, ivan, ivan, newTaskEnd, verificationStart, HITSTaskStatus.InProgress, 4.0),
                CreateLog(authIntegrationTask, ivan, kirill, verificationStart, authIntegrationTask.FinishDate, HITSTaskStatus.OnVerification, 4.0),
                new TaskMovementLog
                {
                    Id = Guid.NewGuid(),
                    Task = authIntegrationTask,
                    Executor = ivan,
                    User = kirill,
                    StartDate = (DateTime)authIntegrationTask.FinishDate!,
                    EndDate = null,
                    WastedTime = string.Empty,
                    Status = HITSTaskStatus.Done
                }
            ]);

            return logs;
        }

        private static DateTime AddWorkingDays(DateTime date, int workingDays, int workingHours = 8)
        {
            DateTime result = date;
            int addedDays = 0;

            while (addedDays < workingDays)
            {
                result = result.AddDays(1);
                if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday || _random.NextDouble() < 0.3)
                    addedDays++;
            }

            return new DateTime(
                result.Year,
                result.Month,
                result.Day,
                _random.Next(9, 22 - _random.Next(1, workingHours + 1)),
                _random.Next(0, 60),
                0
            );
        }

        private static string FormatWorkingTime(TimeSpan duration, double workingHours = 8.0)
        {
            double totalEffectiveHours = duration.TotalDays * (workingHours * 5.0 / 7.0);

            int days = (int)(totalEffectiveHours / workingHours);
            int hours = (int)(totalEffectiveHours % workingHours);
            int minutes = _random.Next(0, 60);

            var parts = new List<string>();
            if (days > 0) parts.Add($"{days}д");
            if (hours > 0) parts.Add($"{hours:00}ч");
            parts.Add($"{minutes:00}мин");

            return string.Join(" ", parts);
        }

        private static TaskMovementLog CreateLog(
            HITSTask task,
            User? executor,
            User user,
            DateTime start,
            DateTime? end,
            HITSTaskStatus status,
            double workingHours = 8.0) =>
        new()
        {
            Id = Guid.NewGuid(),
            Task = task,
            Executor = executor,
            User = user,
            StartDate = start,
            EndDate = end,
            WastedTime = end is not null ? FormatWorkingTime((TimeSpan)(end - start), workingHours) : "-",
            Status = status
        };
    }
}
