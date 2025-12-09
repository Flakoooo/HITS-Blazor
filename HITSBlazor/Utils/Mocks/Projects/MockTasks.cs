using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockTasks
    {
        public static string Task0Id { get; } = "0";
        public static string Task1Id { get; } = "1";
        public static string Task2Id { get; } = "2";
        public static string Task3Id { get; } = "3";
        public static string Task4Id { get; } = "4";
        public static string Task5Id { get; } = "5";
        public static string Task6Id { get; } = "6";
        public static string Task7Id { get; } = "7";
        public static string Task8Id { get; } = "8";
        public static string Task9Id { get; } = "9";
        public static string Task10Id { get; } = "10";
        public static string Task11Id { get; } = "11";
        public static string Task12Id { get; } = "12";
        public static string Task14Id { get; } = "14";
        public static string Task15Id { get; } = "15";
        public static string Task16Id { get; } = "16";

        public static List<Models.Projects.Entities.Task> GetMockTasks()
        {
            var usersMocks = MockUsers.GetMockUsers();
            var tagsMocks = MockTags.GetMockTags(); 

            return
            [
                new Models.Projects.Entities.Task
            {
                Id = Task0Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Разработка административной панели управления задачами",
                Description = "Необходимо создать административную панель для управления задачами, включая просмотр, создание, редактирование и удаление задач",
                Initiator = usersMocks[0],
                Executor = null,
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Tags = [tagsMocks[0], tagsMocks[1], tagsMocks[5]],
                Status = Models.Projects.Enums.TaskStatus.NewTask
            },
            new Models.Projects.Entities.Task
            {
                Id = Task1Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Добавление функционала перетаскивания",
                Description = "Расширить функционал системы задач, добавив возможность перетаскивать задачи для изменения их приоритета или порядка выполнения",
                Initiator = usersMocks[1],
                Executor = null,
                WorkHour = 1,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[5]],
                Status = TaskStatus.NewTask
            },
            new Models.Projects.Entities.Task
            {
                Id = Task2Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Интеграция с системой уведомлений",
                Description = "Настроить интеграцию с системой уведомлений для отправки уведомлений о новых задачах, изменении статусов и прочих событиях",
                Initiator = usersMocks[2],
                Executor = null,
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1], tagsMocks[6]],
                Status = TaskStatus.NewTask
            },
            new Models.Projects.Entities.Task
            {
                Id = Task3Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Улучшение производительности",
                Description = "Провести оптимизацию работы с базой данных и алгоритмов для обеспечения быстрой работы системы даже при большом объеме задач",
                Initiator = usersMocks[0],
                Executor = null,
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1], tagsMocks[8]],
                Status = TaskStatus.NewTask
            },
            new Models.Projects.Entities.Task
            {
                Id = Task4Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Внедрение системы фильтрации и поиска задач",
                Description = "Добавить возможность фильтрации и поиска задач по различным критериям для удобства работы пользователей",
                Initiator = usersMocks[1],
                Executor = usersMocks[3],
                WorkHour = 1,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[1], tagsMocks[5]],
                Status = TaskStatus.OnModification,
                LeaderComment = "Котята, нужно прибраться!"
            },
            new Models.Projects.Entities.Task
            {
                Id = Task5Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Создание отчетности и статистики по выполненным задачам",
                Description = "Разработать модуль для формирования отчетов и статистики по выполненным задачам в удобном для анализа формате",
                Initiator = usersMocks[4],
                Executor = usersMocks[0],
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1], tagsMocks[9]],
                Status = TaskStatus.OnModification
            },
            new Models.Projects.Entities.Task
            {
                Id = Task6Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Разработка механизма повторяющихся задач и напоминаний",
                Description = "Добавить возможность создания повторяющихся задач и настройки напоминаний о них",
                Initiator = usersMocks[0],
                Executor = usersMocks[2],
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[1], tagsMocks[6]],
                Status = TaskStatus.InProgress
            },
            new Models.Projects.Entities.Task
            {
                Id = Task7Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Интеграция с календарем",
                Description = "Настроить интеграцию с календарем для отображения задач в виде событий и синхронизации данных",
                Initiator = usersMocks[0],
                Executor = usersMocks[1],
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[7]],
                Status = TaskStatus.InProgress
            },
            new Models.Projects.Entities.Task
            {
                Id = Task8Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Добавление комментариев к задачам",
                Description = "Расширить функционал задач путем добавления комментариев, обсуждений и возможности прикрепления файлов",
                Initiator = usersMocks[1],
                Executor = usersMocks[1],
                WorkHour = 1,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[1]],
                Status = TaskStatus.OnVerification
            },
            new Models.Projects.Entities.Task
            {
                Id = Task9Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Интеграция с системой аутентификации и авторизации",
                Description = "Настроить интеграцию с системой аутентификации и авторизации пользователей для защиты данных и контроля доступа",
                Initiator = usersMocks[0],
                Executor = usersMocks[0],
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                FinishDate = new DateTime(2024, 1, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1], tagsMocks[7], tagsMocks[10]],
                Status = TaskStatus.Done
            },
            new Models.Projects.Entities.Task
            {
                Id = Task10Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Разработка темной темы интерфейса",
                Description = "Создать альтернативную темную тему интерфейса для удобства пользователей",
                Initiator = usersMocks[1],
                Executor = usersMocks[1],
                WorkHour = 1,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                FinishDate = new DateTime(2024, 1, 22, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[11], tagsMocks[5]],
                Status = TaskStatus.Done
            },
            new Models.Projects.Entities.Task
            {
                Id = Task11Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Добавление возможности прикрепления файлов",
                Description = "Расширить функционал задач с возможностью прикрепления файлов, изображений и документов",
                Initiator = usersMocks[0],
                Executor = usersMocks[2],
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                FinishDate = new DateTime(2024, 1, 21, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1]],
                Status = TaskStatus.Done
            },
            new Models.Projects.Entities.Task
            {
                Id = Task12Id,
                SprintId = "3",
                ProjectId = "0",
                Name = "Разработка системы шаблонов задач",
                Description = "Создать возможность создания и использования шаблонов задач для быстрого добавления типовых задач",
                Initiator = usersMocks[2],
                Executor = usersMocks[0],
                WorkHour = 3,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                FinishDate = new DateTime(2024, 1, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1]],
                Status = TaskStatus.Done
            },
            new Models.Projects.Entities.Task
            {
                Id = Task14Id,
                SprintId = null,
                ProjectId = "0",
                Position = 1,
                Name = "Разработка мобильного приложения для работы с задачами",
                Description = "Создание мобильного приложения для удобной работы с задачами на мобильных устройствах",
                Initiator = usersMocks[1],
                Executor = null,
                WorkHour = 8,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[0], tagsMocks[5]],
                Status = TaskStatus.InBackLog
            },
            new Models.Projects.Entities.Task
            {
                Id = Task15Id,
                SprintId = null,
                ProjectId = "0",
                Position = 2,
                Name = "Интеграция с системой управления версиями",
                Description = "Настроить интеграцию с системой управления версиями для контроля изменений и хранения истории задач",
                Initiator = usersMocks[2],
                Executor = null,
                WorkHour = 4,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1], tagsMocks[7]],
                Status = TaskStatus.InBackLog
            },
            new Models.Projects.Entities.Task
            {
                Id = Task16Id,
                SprintId = null,
                ProjectId = "0",
                Position = 3,
                Name = "Внедрение системы контроля доступа ",
                Description = "Настройка системы контроля доступа и ролевой модели для управления правами пользователей и доступом к функционалу",
                Initiator = usersMocks[4],
                Executor = null,
                WorkHour = 2,
                StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(AppSettings.DateFormats.Iso8601),
                Tags = [tagsMocks[1], tagsMocks[10]],
                Status = TaskStatus.InBackLog
            }
            ];
        }

        // Вспомогательные методы
        public static Models.Projects.Entities.Task? GetTaskById(string id)
            => GetMockTasks().FirstOrDefault(t => t.Id == id);

        public static List<Models.Projects.Entities.Task> GetTasksByProjectId(string projectId)
            => [.. GetMockTasks().Where(t => t.ProjectId == projectId)];

        public static List<Models.Projects.Entities.Task> GetTasksBySprintId(string sprintId)
            => [.. GetMockTasks().Where(t => t.SprintId == sprintId)];

        public static List<Models.Projects.Entities.Task> GetTasksWithoutSprint()
            => [.. GetMockTasks().Where(t => string.IsNullOrEmpty(t.SprintId))];

        public static List<Models.Projects.Entities.Task> GetTasksByExecutor(string executorId)
            => [.. GetMockTasks().Where(t => t.Executor?.Id == executorId)];

        public static List<Models.Projects.Entities.Task> GetTasksByInitiator(string initiatorId)
            => [.. GetMockTasks().Where(t => t.Initiator.Id == initiatorId)];

        public static List<Models.Projects.Entities.Task> GetTasksByStatus(TaskStatus status)
            => [.. GetMockTasks().Where(t => t.Status == status)];

        public static List<Models.Projects.Entities.Task> GetTasksByTag(string tagId)
            => [.. GetMockTasks().Where(t => t.Tags.Any(tag => tag.Id == tagId))];

        public static List<Models.Projects.Entities.Task> GetOverdueTasks()
        {
            var now = DateTime.UtcNow;
            return [.. GetMockTasks().Where(t =>
            t.Status != TaskStatus.Done &&
            t.FinishDate != null &&
            DateTime.Parse(t.FinishDate) < now)];
        }

        public static List<Models.Projects.Entities.Task> GetTasksDueSoon(int daysThreshold = 3)
        {
            var now = DateTime.UtcNow;
            var thresholdDate = now.AddDays(daysThreshold);

            return [.. GetMockTasks().Where(t =>
            t.Status != TaskStatus.Done &&
            t.FinishDate != null &&
            DateTime.Parse(t.FinishDate) >= now &&
            DateTime.Parse(t.FinishDate) <= thresholdDate)];
        }

        public static List<Models.Projects.Entities.Task> GetTasksInProgress()
            => GetTasksByStatus(TaskStatus.InProgress)
                .Concat(GetTasksByStatus(TaskStatus.OnVerification))
                .Concat(GetTasksByStatus(TaskStatus.OnModification))
                .ToList();

        public static int GetTotalWorkHoursForProject(string projectId)
            => GetTasksByProjectId(projectId).Sum(t => t.WorkHour);

        public static int GetCompletedWorkHoursForProject(string projectId)
            => GetTasksByProjectId(projectId)
                .Where(t => t.Status == TaskStatus.Done)
                .Sum(t => t.WorkHour);

        public static double GetProjectCompletionPercentage(string projectId)
        {
            var totalHours = GetTotalWorkHoursForProject(projectId);
            if (totalHours == 0) return 0;

            var completedHours = GetCompletedWorkHoursForProject(projectId);
            return (double)completedHours / totalHours * 100;
        }

        public static List<User> GetBusiestUsers(string projectId)
        {
            var tasks = GetTasksByProjectId(projectId)
                .Where(t => t.Executor != null && t.Status != TaskStatus.Done);

            return tasks
                .GroupBy(t => t.Executor!)
                .Select(g => new
                {
                    User = g.Key,
                    WorkHours = g.Sum(t => t.WorkHour),
                    TaskCount = g.Count()
                })
                .OrderByDescending(x => x.WorkHours)
                .Select(x => x.User)
                .ToList();
        }

        public static List<Models.Projects.Entities.Task> GetTasksForUserDashboard(string userId)
        {
            var userTasks = GetMockTasks()
                .Where(t => t.Executor?.Id == userId || t.Initiator.Id == userId)
                .OrderByDescending(t =>
                    t.Status == TaskStatus.InProgress ? 3 :
                    t.Status == TaskStatus.OnVerification ? 2 :
                    t.Status == TaskStatus.OnModification ? 1 : 0)
                .ThenBy(t => t.StartDate)
                .ToList();

            return userTasks;
        }

        public static Dictionary<TaskStatus, int> GetTaskStatistics(string projectId)
        {
            var tasks = GetTasksByProjectId(projectId);

            return Enum.GetValues<TaskStatus>()
                .Cast<TaskStatus>()
                .ToDictionary(
                    status => status,
                    status => tasks.Count(t => t.Status == status));
        }
    }
}
