using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;
using System.Globalization;

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

            DateTime startDate = DateTime.ParseExact(
                authIntegrationTask.StartDate, Settings.DateFormat, null, DateTimeStyles.AdjustToUniversal);
            DateTime finishDate = DateTime.ParseExact(
                authIntegrationTask.FinishDate!, Settings.DateFormat, null, DateTimeStyles.AdjustToUniversal);

            var logs = new List<TaskMovementLog>();

            var newTaskEnd = AddWorkingDays(startDate, _random.Next(1, 3), 4);
            logs.Add(CreateLog(authIntegrationTask, null, kirill, startDate, newTaskEnd, Models.Projects.Enums.TaskStatus.NewTask, 4.0));

            var verificationStart = newTaskEnd + ((finishDate - newTaskEnd) * 0.7);

            logs.AddRange([
                CreateLog(authIntegrationTask, ivan, ivan, newTaskEnd, verificationStart, Models.Projects.Enums.TaskStatus.InProgress, 4.0),
                CreateLog(authIntegrationTask, ivan, kirill, verificationStart, finishDate, Models.Projects.Enums.TaskStatus.OnVerification, 4.0),
                new TaskMovementLog
                {
                    Id = Guid.NewGuid(),
                    Task = authIntegrationTask,
                    Executor = ivan,
                    User = kirill,
                    StartDate = finishDate.ToString(Settings.DateFormat),
                    EndDate = string.Empty,
                    WastedTime = string.Empty,
                    Status = Models.Projects.Enums.TaskStatus.Done
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
            Models.Projects.Entities.Task task,
            User? executor,
            User user,
            DateTime start,
            DateTime end,
            Models.Projects.Enums.TaskStatus status,
            double workingHours = 8.0) =>
        new()
        {
            Id = Guid.NewGuid(),
            Task = task,
            Executor = executor,
            User = user,
            StartDate = start.ToString(Settings.DateFormat),
            EndDate = end.ToString(Settings.DateFormat),
            WastedTime = FormatWorkingTime(end - start, workingHours),
            Status = status
        };
    }
}
