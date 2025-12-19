using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockSprints
    {
        private static readonly List<Sprint> _sprints = CreateSprints();
        private static readonly List<Models.Projects.Entities.Task> _tasks = CreateTasks();

        public static Guid Sprint1Id { get; }
        public static Guid Sprint2Id { get; }
        public static Guid Sprint3Id { get; }
        public static Guid ScrumDesignSprintId { get; } 
        public static Guid AuthIntegrationTaskId { get; }
        public static Guid BlackThemeTaskId { get; }
        public static Guid FileUploadTaskId { get; }
        public static Guid TaskTemplatesTaskId { get; }

        private static int SumSprintDate(int sprintCount, bool isFinishDate, int sprintDurationDays = 7)
            => isFinishDate
            ? sprintCount * 1 + ++sprintCount * sprintDurationDays
            : sprintCount * (sprintDurationDays + 1);


        private static List<Models.Projects.Entities.Task> CreateTasks()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;
            var winrit = MockUsers.GetUserById(MockUsers.WinritId)!;

            var frontendTag = MockTags.GetTagById(MockTags.FrontendId)!;
            var backendTag = MockTags.GetTagById(MockTags.BackendId)!;
            var uiuxTag = MockTags.GetTagById(MockTags.UIUXId)!;
            var notificationTag = MockTags.GetTagById(MockTags.NotificationId)!;
            var integrationTag = MockTags.GetTagById(MockTags.IntegrationId)!;
            var securityTag = MockTags.GetTagById(MockTags.SecurityId)!;

            var tags015 = new List<Tag>() { frontendTag, backendTag, uiuxTag };

            return
            [
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка административной панели управления задачами",
                    Description = "Необходимо создать административную панель для управления задачами, включая просмотр, создание, редактирование и удаление задач",
                    Initiator = kirill,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [.. tags015],
                    Status = Models.Projects.Enums.TaskStatus.NewTask
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Добавление функционала перетаскивания",
                    Description = "Расширить функционал системы задач, добавив возможность перетаскивать задачи для изменения их приоритета или порядка выполнения",
                    Initiator = ivan,
                    Executor = null,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, uiuxTag],
                    Status = Models.Projects.Enums.TaskStatus.NewTask
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Интеграция с системой уведомлений",
                    Description = "Настроить интеграцию с системой уведомлений для отправки уведомлений о новых задачах, изменении статусов и прочих событиях",
                    Initiator = manager,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, notificationTag],
                    Status = Models.Projects.Enums.TaskStatus.NewTask
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Улучшение производительности",
                    Description = "Провести оптимизацию работы с базой данных и алгоритмов для обеспечения быстрой работы системы даже при большом объеме задач",
                    Initiator = kirill,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, MockTags.GetTagById(MockTags.OptimizationId)!],
                    Status = Models.Projects.Enums.TaskStatus.NewTask
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Внедрение системы фильтрации и поиска задач",
                    Description = "Добавить возможность фильтрации и поиска задач по различным критериям для удобства работы пользователей",
                    Initiator = ivan,
                    Executor = winrit,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [.. tags015],
                    Status = Models.Projects.Enums.TaskStatus.OnModification,
                    LeaderComment = "Котята, нужно прибраться!"
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Создание отчетности и статистики по выполненным задачам",
                    Description = "Разработать модуль для формирования отчетов и статистики по выполненным задачам в удобном для анализа формате",
                    Initiator = MockUsers.GetUserById(MockUsers.OwnerId)!,
                    Executor = kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, MockTags.GetTagById(MockTags.StatisticId)!],
                    Status = Models.Projects.Enums.TaskStatus.OnModification
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка механизма повторяющихся задач и напоминаний",
                    Description = "Добавить возможность создания повторяющихся задач и настройки напоминаний о них",
                    Initiator = kirill,
                    Executor = manager,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, backendTag, notificationTag],
                    Status = Models.Projects.Enums.TaskStatus.InProgress
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Интеграция с календарем",
                    Description = "Настроить интеграцию с календарем для отображения задач в виде событий и синхронизации данных",
                    Initiator = kirill,
                    Executor = ivan,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, integrationTag],
                    Status = Models.Projects.Enums.TaskStatus.InProgress
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Добавление комментариев к задачам",
                    Description = "Расширить функционал задач путем добавления комментариев, обсуждений и возможности прикрепления файлов",
                    Initiator = ivan,
                    Executor = ivan,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, backendTag],
                    Status = Models.Projects.Enums.TaskStatus.OnVerification
                },
                new Models.Projects.Entities.Task
                {
                    Id = AuthIntegrationTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Интеграция с системой аутентификации и авторизации",
                    Description = "Настроить интеграцию с системой аутентификации и авторизации пользователей для защиты данных и контроля доступа",
                    Initiator = kirill,
                    Executor = kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = new DateTime(2024, 1, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, integrationTag, securityTag],
                    Status = Models.Projects.Enums.TaskStatus.Done
                },
                new Models.Projects.Entities.Task
                {
                    Id = BlackThemeTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка темной темы интерфейса",
                    Description = "Создать альтернативную темную тему интерфейса для удобства пользователей",
                    Initiator = ivan,
                    Executor = ivan,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = new DateTime(2024, 1, 22, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, MockTags.GetTagById(MockTags.DesignId)!, uiuxTag],
                    Status = Models.Projects.Enums.TaskStatus.Done
                },
                new Models.Projects.Entities.Task
                {
                    Id = FileUploadTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Добавление возможности прикрепления файлов",
                    Description = "Расширить функционал задач с возможностью прикрепления файлов, изображений и документов",
                    Initiator = kirill,
                    Executor = manager,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = new DateTime(2024, 1, 21, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag],
                    Status = Models.Projects.Enums.TaskStatus.Done
                },
                new Models.Projects.Entities.Task
                {
                    Id = TaskTemplatesTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка системы шаблонов задач",
                    Description = "Создать возможность создания и использования шаблонов задач для быстрого добавления типовых задач",
                    Initiator = manager,
                    Executor = kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = new DateTime(2024, 1, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag],
                    Status = Models.Projects.Enums.TaskStatus.Done
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Guid.Empty,
                    ProjectId = MockProjects.ChatBotId,
                    Position = 1,
                    Name = "Разработка мобильного приложения для работы с задачами",
                    Description = "Создание мобильного приложения для удобной работы с задачами на мобильных устройствах",
                    Initiator = ivan,
                    Executor = null,
                    WorkHour = 8,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, uiuxTag],
                    Status = Models.Projects.Enums.TaskStatus.InBackLog
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Guid.Empty,
                    ProjectId = MockProjects.ChatBotId,
                    Position = 2,
                    Name = "Интеграция с системой управления версиями",
                    Description = "Настроить интеграцию с системой управления версиями для контроля изменений и хранения истории задач",
                    Initiator = manager,
                    Executor = null,
                    WorkHour = 4,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, integrationTag],
                    Status = Models.Projects.Enums.TaskStatus.InBackLog
                },
                new Models.Projects.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    SprintId = Guid.Empty,
                    ProjectId = MockProjects.ChatBotId,
                    Position = 3,
                    Name = "Внедрение системы контроля доступа ",
                    Description = "Настройка системы контроля доступа и ролевой модели для управления правами пользователей и доступом к функционалу",
                    Initiator = winrit,
                    Executor = null,
                    WorkHour = 2,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, securityTag],
                    Status = Models.Projects.Enums.TaskStatus.InBackLog
                }
            ];
        }

        private static List<Sprint> CreateSprints()
        {
            var firstSprintDate = new DateTime(2023, 12, 26, 11, 2, 17, DateTimeKind.Utc);

            return
            [
                new Sprint
                {
                    Id = Sprint1Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Спринт 1",
                    Goal = "Цель 1",
                    Report = "Отчет 1",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(0, false)).ToString(Settings.DateFormat),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(0, true)).ToString(Settings.DateFormat),
                    WorkingHours = 15,
                    Status = SprintStatus.DONE,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Sprint1Id)]
                },
                new Sprint
                {
                    Id = Sprint2Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Спринт 2",
                    Goal = "Цель 2",
                    Report = "Отчет 2",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(1, false)).ToString(Settings.DateFormat),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(1, true)).ToString(Settings.DateFormat),
                    WorkingHours = 15,
                    Status = SprintStatus.DONE,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Sprint2Id)]
                },
                new Sprint
                {
                    Id = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Спринт 3",
                    Goal = "Цель 3",
                    Report = "Отчет 3",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(2, false)).ToString(Settings.DateFormat),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(2, true)).ToString(Settings.DateFormat),
                    WorkingHours = 15,
                    Status = SprintStatus.DONE,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Sprint3Id)]
                },
                new Sprint
                {
                    Id = ScrumDesignSprintId,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Проектировка скрама",
                    Goal = "Цель 4",
                    Report = "Отчет 4",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(3, false)).ToString(Settings.DateFormat),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(3, true)).ToString(Settings.DateFormat),
                    WorkingHours = 20,
                    Status = SprintStatus.ACTIVE,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Guid.Empty)] // последний спринт
                }
            ];
        }

        public static Models.Projects.Entities.Task? GetTaskById(Guid taskId) =>
            _tasks.FirstOrDefault(t => t.Id == taskId);
    }
}
