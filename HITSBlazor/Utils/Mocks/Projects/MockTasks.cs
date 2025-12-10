using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockTasks
    {
        private static readonly List<Models.Projects.Entities.Task> _tasks = CreateTasks();

        public static string AdminPanelId { get; } = Guid.NewGuid().ToString();
        public static string MoveFunctionalId { get; } = Guid.NewGuid().ToString();
        public static string NotifyIntegrationId { get; } = Guid.NewGuid().ToString();
        public static string OptimizationId { get; } = Guid.NewGuid().ToString();
        public static string AddFiltrationId { get; } = Guid.NewGuid().ToString();
        public static string CreateStatisticId { get; } = Guid.NewGuid().ToString();
        public static string CreateNotifyId { get; } = Guid.NewGuid().ToString();
        public static string CalendarIntegrationId { get; } = Guid.NewGuid().ToString();
        public static string AddCommentsId { get; } = Guid.NewGuid().ToString();
        public static string AuthIntegrationId { get; } = Guid.NewGuid().ToString();
        public static string BlackThemeId { get; } = Guid.NewGuid().ToString();
        public static string FileUploadId { get; } = Guid.NewGuid().ToString();
        public static string TaskTemplatesId { get; } = Guid.NewGuid().ToString();
        public static string MobileAppId { get; } = Guid.NewGuid().ToString();
        public static string VersionControlId { get; } = Guid.NewGuid().ToString();
        public static string AccessControlId { get; } = Guid.NewGuid().ToString();

        private static List<Models.Projects.Entities.Task> CreateTasks()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;
            var winrit = MockUsers.GetUserById(MockUsers.WinritId)!;

            var frontendTag = MockTags.GetTagById(MockTags.FrontendId)!;
            var backendTag = MockTags.GetTagById(MockTags.BackendId)!;
            var uiuxTag = MockTags.GetTagById(MockTags.UIUXId)!;
            var notificationTag = MockTags.GetTagById(MockTags.NotificationId);
            var integrationTag = MockTags.GetTagById(MockTags.IntegrationId);
            var securityTag = MockTags.GetTagById(MockTags.SecurityId);

            var tags015 = new List<Tag>() { frontendTag, backendTag, uiuxTag };

            return
            [
                new Models.Projects.Entities.Task
                {
                    Id = AdminPanelId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = MoveFunctionalId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = NotifyIntegrationId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = OptimizationId,
                    SprintId = "3",
                    ProjectId = "0",
                    Name = "Улучшение производительности",
                    Description = "Провести оптимизацию работы с базой данных и алгоритмов для обеспечения быстрой работы системы даже при большом объеме задач",
                    Initiator = kirill,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, MockTags.GetTagById(MockTags.OptimizationId)],
                    Status = Models.Projects.Enums.TaskStatus.NewTask
                },
                new Models.Projects.Entities.Task
                {
                    Id = AddFiltrationId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = CreateStatisticId,
                    SprintId = "3",
                    ProjectId = "0",
                    Name = "Создание отчетности и статистики по выполненным задачам",
                    Description = "Разработать модуль для формирования отчетов и статистики по выполненным задачам в удобном для анализа формате",
                    Initiator = MockUsers.GetUserById(MockUsers.OwnerId)!,
                    Executor = kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [backendTag, MockTags.GetTagById(MockTags.StatisticId)],
                    Status = Models.Projects.Enums.TaskStatus.OnModification
                },
                new Models.Projects.Entities.Task
                {
                    Id = CreateNotifyId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = CalendarIntegrationId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = AddCommentsId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = AuthIntegrationId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = BlackThemeId,
                    SprintId = "3",
                    ProjectId = "0",
                    Name = "Разработка темной темы интерфейса",
                    Description = "Создать альтернативную темную тему интерфейса для удобства пользователей",
                    Initiator = ivan,
                    Executor = ivan,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = new DateTime(2024, 1, 22, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Tags = [frontendTag, MockTags.GetTagById(MockTags.DesignId), uiuxTag],
                    Status = Models.Projects.Enums.TaskStatus.Done
                },
                new Models.Projects.Entities.Task
                {
                    Id = FileUploadId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = TaskTemplatesId,
                    SprintId = "3",
                    ProjectId = "0",
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
                    Id = MobileAppId,
                    SprintId = null,
                    ProjectId = "0",
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
                    Id = VersionControlId,
                    SprintId = null,
                    ProjectId = "0",
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
                    Id = AccessControlId,
                    SprintId = null,
                    ProjectId = "0",
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

        public static Models.Projects.Entities.Task? GetTaskById(string id)
            => _tasks.FirstOrDefault(t => t.Id == id);
    }
}
