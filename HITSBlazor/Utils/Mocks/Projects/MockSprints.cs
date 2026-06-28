using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockSprints
    {
        public static Guid Sprint1Id { get; } = Guid.NewGuid();
        public static Guid Sprint2Id { get; } = Guid.NewGuid();
        public static Guid Sprint3Id { get; } = Guid.NewGuid();
        public static Guid ScrumDesignSprintId { get; } = Guid.NewGuid();
        public static Guid AuthIntegrationTaskId { get; } = Guid.NewGuid();
        public static Guid BlackThemeTaskId { get; } = Guid.NewGuid();
        public static Guid FileUploadTaskId { get; } = Guid.NewGuid();
        public static Guid TaskTemplatesTaskId { get; } = Guid.NewGuid();

        private class TaskHistory
        {
            public required HITSTask Task { get; set; }
            public Guid SprintId { get; set; }
            public HITSTaskStatus Status { get; set; }
            public User? ExecutorId { get; set; }
        }

        private static readonly List<HITSTask> _tasks = CreateTasks();
        private static readonly List<Sprint> _sprints = CreateSprints();
        private static readonly List<TaskHistory> _taskHistory = new List<TaskHistory>();

        private static int SumSprintDate(int sprintCount, bool isFinishDate, int sprintDurationDays = 7)
            => isFinishDate
            ? sprintCount * 1 + ++sprintCount * sprintDurationDays
            : sprintCount * (sprintDurationDays + 1);


        private static List<HITSTask> CreateTasks()
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

            var chatBotProject = MockProjects.GetProjectById(MockProjects.ChatBotId);

            var tags015 = new List<Tag>() { frontendTag, backendTag, uiuxTag };

            return
            [
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка административной панели управления задачами",
                    Description = "Необходимо создать административную панель для управления задачами, включая просмотр, создание, редактирование и удаление задач",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User {
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [.. tags015],
                    Status = HITSTaskStatus.NewTask
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Добавление функционала перетаскивания",
                    Description = "Расширить функционал системы задач, добавив возможность перетаскивать задачи для изменения их приоритета или порядка выполнения",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    Executor = null,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [frontendTag, uiuxTag],
                    Status = HITSTaskStatus.NewTask
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Интеграция с системой уведомлений",
                    Description = "Настроить интеграцию с системой уведомлений для отправки уведомлений о новых задачах, изменении статусов и прочих событиях",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? manager,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag, notificationTag],
                    Status = HITSTaskStatus.NewTask
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Улучшение производительности",
                    Description = "Провести оптимизацию работы с базой данных и алгоритмов для обеспечения быстрой работы системы даже при большом объеме задач",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    Executor = null,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag, MockTags.GetTagById(MockTags.OptimizationId)!],
                    Status = HITSTaskStatus.NewTask
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Внедрение системы фильтрации и поиска задач",
                    Description = "Добавить возможность фильтрации и поиска задач по различным критериям для удобства работы пользователей",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? winrit,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [.. tags015],
                    Status = HITSTaskStatus.OnModification,
                    LeaderComment = "Котята, нужно прибраться!"
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Создание отчетности и статистики по выполненным задачам",
                    Description = "Разработать модуль для формирования отчетов и статистики по выполненным задачам в удобном для анализа формате",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? MockUsers.GetUserById(MockUsers.OwnerId)!,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag, MockTags.GetTagById(MockTags.StatisticId)!],
                    Status = HITSTaskStatus.OnModification
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка механизма повторяющихся задач и напоминаний",
                    Description = "Добавить возможность создания повторяющихся задач и настройки напоминаний о них",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ??  manager,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [frontendTag, backendTag, notificationTag],
                    Status = HITSTaskStatus.InProgress
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Интеграция с календарем",
                    Description = "Настроить интеграцию с календарем для отображения задач в виде событий и синхронизации данных",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [frontendTag, integrationTag],
                    Status = HITSTaskStatus.InProgress
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Добавление комментариев к задачам",
                    Description = "Расширить функционал задач путем добавления комментариев, обсуждений и возможности прикрепления файлов",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ??  ivan,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [frontendTag, backendTag],
                    Status = HITSTaskStatus.OnVerification
                },
                new HITSTask
                {
                    Id = AuthIntegrationTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Интеграция с системой аутентификации и авторизации",
                    Description = "Настроить интеграцию с системой аутентификации и авторизации пользователей для защиты данных и контроля доступа",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    FinishDate = new DateTime(2024, 1, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag, integrationTag, securityTag],
                    Status = HITSTaskStatus.Done
                },
                new HITSTask
                {
                    Id = BlackThemeTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка темной темы интерфейса",
                    Description = "Создать альтернативную темную тему интерфейса для удобства пользователей",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    WorkHour = 1,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    FinishDate = new DateTime(2024, 1, 22, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [frontendTag, MockTags.GetTagById(MockTags.DesignId)!, uiuxTag],
                    Status = HITSTaskStatus.Done
                },
                new HITSTask
                {
                    Id = FileUploadTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Добавление возможности прикрепления файлов",
                    Description = "Расширить функционал задач с возможностью прикрепления файлов, изображений и документов",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? manager,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    FinishDate = new DateTime(2024, 1, 21, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag],
                    Status = HITSTaskStatus.Done
                },
                new HITSTask
                {
                    Id = TaskTemplatesTaskId,
                    SprintId = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Разработка системы шаблонов задач",
                    Description = "Создать возможность создания и использования шаблонов задач для быстрого добавления типовых задач",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? manager,
                    Executor = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? kirill,
                    WorkHour = 3,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    FinishDate = new DateTime(2024, 1, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag],
                    Status = HITSTaskStatus.Done
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Guid.Empty,
                    ProjectId = MockProjects.ChatBotId,
                    Position = 1,
                    Name = "Разработка мобильного приложения для работы с задачами",
                    Description = "Создание мобильного приложения для удобной работы с задачами на мобильных устройствах",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.Member)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? ivan,
                    Executor = null,
                    WorkHour = 8,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [frontendTag, uiuxTag],
                    Status = HITSTaskStatus.InBackLog
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Guid.Empty,
                    ProjectId = MockProjects.ChatBotId,
                    Position = 2,
                    Name = "Интеграция с системой управления версиями",
                    Description = "Настроить интеграцию с системой управления версиями для контроля изменений и хранения истории задач",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? manager,
                    Executor = null,
                    WorkHour = 4,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag, integrationTag],
                    Status = HITSTaskStatus.InBackLog
                },
                new HITSTask
                {
                    Id = Guid.NewGuid(),
                    SprintId = Guid.Empty,
                    ProjectId = MockProjects.ChatBotId,
                    Position = 3,
                    Name = "Внедрение системы контроля доступа ",
                    Description = "Настройка системы контроля доступа и ролевой модели для управления правами пользователей и доступом к функционалу",
                    Initiator = chatBotProject?.Members
                        .Where(m => m.ProjectRole is ProjectMemberRole.TeamLeader)
                        .Select(m => new User{
                            Id = m.UserId,
                            Email = m.Email,
                            FirstName = m.FirstName,
                            LastName = m.LastName
                        })
                        .FirstOrDefault() ?? winrit,
                    Executor = null,
                    WorkHour = 2,
                    StartDate = new DateTime(2023, 9, 25, 11, 2, 17, DateTimeKind.Utc),
                    Tags = [backendTag, securityTag],
                    Status = HITSTaskStatus.InBackLog
                }
            ];
        }

        private static List<Sprint> CreateSprints()
        {
            var firstSprintDate = new DateOnly(2023, 12, 26);

            return
            [
                new Sprint
                {
                    Id = Sprint1Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Спринт 1",
                    Goal = "Цель 1",
                    Report = "Отчет 1",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(0, false)),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(0, true)),
                    WorkingHours = 15,
                    Status = SprintStatus.Done,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Sprint1Id)]
                },
                new Sprint
                {
                    Id = Sprint2Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Спринт 2",
                    Goal = "Цель 2",
                    Report = "Отчет 2",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(1, false)),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(1, true)),
                    WorkingHours = 15,
                    Status = SprintStatus.Done,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Sprint2Id)]
                },
                new Sprint
                {
                    Id = Sprint3Id,
                    ProjectId = MockProjects.ChatBotId,
                    Name = "Спринт 3",
                    Goal = "Цель 3",
                    Report = "Отчет 3",
                    StartDate = firstSprintDate.AddDays(SumSprintDate(2, false)),
                    FinishDate = firstSprintDate.AddDays(SumSprintDate(2, true)),
                    WorkingHours = _tasks.Where(t => t.SprintId == Sprint3Id).Sum(t => t.WorkHour),
                    Status = SprintStatus.Active,
                    Tasks = [.. _tasks.Where(t => t.SprintId == Sprint3Id)]
                },
                //new Sprint
                //{
                //    Id = ScrumDesignSprintId,
                //    ProjectId = MockProjects.ChatBotId,
                //    Name = "Проектировка скрама",
                //    Goal = "Цель 4",
                //    Report = "Отчет 4",
                //    StartDate = firstSprintDate.AddDays(SumSprintDate(3, false)),
                //    FinishDate = firstSprintDate.AddDays(SumSprintDate(3, true)),
                //    WorkingHours = 20,
                //    Status = SprintStatus.Active,
                //    Tasks = [.. _tasks.Where(t => t.SprintId == Guid.Empty)]
                //}
            ];
        }

        private static void UpdateTaskBackLogPosition(Guid projectId)
        {
            int position = 1;
            foreach (var task in _tasks.Where(t => t.ProjectId == projectId && t.Status is HITSTaskStatus.InBackLog).OrderBy(t => t.Position))
                task.Position = position++;
        }

        public static List<Sprint> GetAllMockSprints(Guid? projectId = null)
        {
            if (projectId.HasValue)
                return _sprints.Where(s => s.ProjectId == projectId).ToList();
            else
                return _sprints.ToList();
        }

        public static ListDataResponse<Sprint> GetSprintsByProjectId(
            Guid projectId, 
            int page, 
            int pageSize = 20,
            string? searchText = null
        )
        {
            var query = _sprints.Where(s => s.ProjectId == projectId);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(s => s.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<Sprint>(count, query.ToList());
        }

        public static Sprint? GetActiveSprintByProjectId(Guid projectId)
            => _sprints.FirstOrDefault(s => s.ProjectId == projectId && s.Status is SprintStatus.Active);

        public static Sprint? CreateSprint(CreateSprintRequest request)
        {
            var newSprint = new Sprint
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                Name = request.Name,
                Goal = request.Goal,
                StartDate = request.StartDate,
                FinishDate = request.FinishDate,
                WorkingHours = request.WorkingHours,
                Status = SprintStatus.Active,
                Tasks = _tasks.Where(t => request.Tasks.Contains(t.Id)).ToList()
            };

            _sprints.Add(newSprint);

            foreach (var task in _tasks.Where(t => request.Tasks.Contains(t.Id)))
            {
                task.Status = HITSTaskStatus.NewTask;
                task.SprintId = newSprint.Id;
            }    

            return newSprint;
        }

        public static Sprint? UpdateSprint(Guid sprintId, User updateInitiator, UpdateSprintRequest request)
        {
            var sprintForUpdate = _sprints.FirstOrDefault(s => s.Id == sprintId);
            if (sprintForUpdate is null) return null;

            sprintForUpdate.Name = request.Name ?? sprintForUpdate.Name;
            sprintForUpdate.Goal = request.Goal ?? sprintForUpdate.Goal;
            sprintForUpdate.StartDate = request.StartDate ?? sprintForUpdate.StartDate;
            sprintForUpdate.FinishDate = request.FinishDate ?? sprintForUpdate.FinishDate;
            sprintForUpdate.WorkingHours = request.WorkingHours ?? sprintForUpdate.WorkingHours;

            if (sprintForUpdate.Tasks.Count < request.Tasks?.Count)
            {
                foreach (var task in _tasks.Where(t => request.Tasks.Contains(t.Id) && t.Status is HITSTaskStatus.InBackLog))
                {
                    task.Status = HITSTaskStatus.NewTask;
                    task.SprintId = sprintId;
                    UpdateTaskStatus(task.Id, HITSTaskStatus.NewTask, updateInitiator);
                }

                sprintForUpdate.Tasks = _tasks.Where(t => request.Tasks.Contains(t.Id)).ToList();

                UpdateTaskBackLogPosition(sprintForUpdate.ProjectId);
            }

            return sprintForUpdate;
        }

        private static User? CopyUser(User? copy)
        {
            if (copy is null) return null;

            return new User
            {
                Id = copy.Id,
                Email = copy.Email,
                FirstName = copy.FirstName,
                LastName = copy.LastName,
                CreatedAt = copy.CreatedAt,
                Roles = copy.Roles.ToList(),
                Telephone = copy.Telephone,
                StudyGroup = copy.StudyGroup
            };
        }

        public static bool FinishSprint(Guid sprintId, string report, IEnumerable<SprintMarkRequest> marks)
        {
            var currentSprint = _sprints.FirstOrDefault(s => s.Id == sprintId && s.Status is SprintStatus.Active);
            if (currentSprint is null) return false;

            currentSprint.Report = report;

            var completedTasks = new Dictionary<Guid, List<HITSTask>>();

            var sprintTasks = _tasks.Where(t => t.SprintId == sprintId);
            foreach (var task in sprintTasks)
            {
                _taskHistory.Add(new TaskHistory
                {
                    Task = new HITSTask
                    {
                        Id = task.Id,
                        SprintId = task.SprintId,
                        ProjectId = task.ProjectId,
                        Name = task.Name,
                        Description = task.Description,
                        LeaderComment = task.LeaderComment,
                        ExecutorComment = task.ExecutorComment,
                        Initiator = CopyUser(task.Initiator)!,
                        Executor = CopyUser(task.Executor),
                        WorkHour = task.WorkHour,
                        StartDate = task.StartDate,
                        FinishDate = task.FinishDate,
                        Tags = task.Tags.ToList(),
                        Status = task.Status
                    },
                    SprintId = sprintId,
                    Status = task.Status,
                    ExecutorId = task.Executor
                });

                task.SprintId = null;

                if (task.Status is HITSTaskStatus.Done)
                {
                    if (task.Executor is null) continue;

                    if (completedTasks.TryGetValue(task.Executor.Id, out var tasks))
                        tasks.Add(task);
                    else
                        completedTasks.Add(task.Executor.Id, [task]);
                }
                else
                {
                    task.Status = HITSTaskStatus.InBackLog;
                    task.Executor = null;
                }

            }

            UpdateTaskBackLogPosition(currentSprint.ProjectId);

            MockSprintMarks.CreateSprintMarks(
                currentSprint.ProjectId, sprintId, completedTasks, marks
            );

            MockAverageMarks.UpdateProjectMarks(
                currentSprint.ProjectId, completedTasks
            );

            currentSprint.Status = SprintStatus.Done;

            return true;
        }

        public static List<HITSTask> GetMockTasks() => _tasks;

        public static HITSTask? GetTaskById(Guid taskId) =>
            _tasks.FirstOrDefault(t => t.Id == taskId);

        public static ListDataResponse<HITSTask> GetTasksByQueryParams( 
            int page, 
            int pageSize = 40,
            Guid? projectId = null,
            Guid? sprintId = null,
            string? searchText = null,
            HashSet<HITSTaskStatus>? selectedStatuses = null,
            HashSet<Guid>? selectedTags = null,
            HashSet<Guid>? selectedExecutors = null
        )
        {
            IEnumerable<HITSTask> query;
            if (projectId.HasValue && sprintId.HasValue)
            {
                query = _tasks.Where(t => t.ProjectId == projectId && t.SprintId == sprintId).OrderBy(t => t.Position);
            }
            else if (projectId.HasValue)
            {
                query = _tasks.Where(t => t.ProjectId == projectId).OrderBy(t => t.Position);
            }
            else if (sprintId.HasValue)
            {
                var sprint = _sprints.FirstOrDefault(s => s.Id == sprintId.Value);
                if (sprint is not null)
                {
                    if (sprint.Status is SprintStatus.Active)
                    {
                        query = _tasks.Where(t => t.SprintId == sprintId).OrderBy(t => t.Position);
                    }
                    else
                    {
                        query = _taskHistory.Where(th => th.SprintId == sprintId).Select(t => t.Task);
                    }
                }
                else
                {
                    query = _tasks.OrderBy(t => t.Position).AsEnumerable();
                }
            }
            else
            {
                query = _tasks.OrderBy(t => t.Position).AsEnumerable();
            }

            if (selectedExecutors?.Count > 0)
                query = query.Where(t => t.Executor is not null && selectedExecutors.Contains(t.Executor.Id));

            if (selectedStatuses?.Count > 0)
                query = query.Where(t => selectedStatuses.Contains(t.Status));

            if (selectedTags?.Count > 0)
            {
                query = query.Where(t => t.Tags.Any(tag => selectedTags.Contains(tag.Id)));
            }    

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(s => s.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<HITSTask>(count, query.ToList());
        }

        public static bool MemberHasTaskInProgress(Guid sprintId, Guid userId)
            => _tasks.Any(t => 
                t.SprintId == sprintId && t.Executor?.Id == userId && t.Status is HITSTaskStatus.InProgress
            );

        public static HITSTask? CreateTask(CreateTaskRequest request)
        {
            var newTask = new HITSTask
            {
                Id = Guid.NewGuid(),
                SprintId = request.SprintId,
                ProjectId = request.ProjectId,
                Name = request.Name,
                Description = request.Description,
                Initiator = request.Initiator,
                WorkHour = request.WorkHour,
                StartDate = request.StartDate,
                Tags = request.Tags.ToList(),
                Status = request.Status
            };

            if (newTask.Status is HITSTaskStatus.InBackLog)
                newTask.Position = _tasks.Count(t => t.ProjectId == request.ProjectId && t.Status is HITSTaskStatus.InBackLog) + 1;

            _tasks.Add(newTask);
            MockTaskMovementLogs.CreateNewTaskLog(newTask, request.Initiator);

            return newTask;
        }

        public static HITSTask? UpdateTask(Guid taskId, UpdateTaskRequest request)
        {
            var taskForUpdate = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (taskForUpdate is null) return null;

            taskForUpdate.Name = request.Name ?? taskForUpdate.Name;
            taskForUpdate.Description = request.Description ?? taskForUpdate.Description;
            taskForUpdate.Tags = request.Tags?.ToList() ?? taskForUpdate.Tags;
            taskForUpdate.WorkHour = request.WorkHour ?? taskForUpdate.WorkHour;

            return taskForUpdate;
        }

        public static bool UpdateTaskComment(Guid taskId, string comment, ProjectMemberRole executorRole)
        {
            var taskForUpdate = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (taskForUpdate is null) return false;

            if (executorRole is ProjectMemberRole.TeamLeader)
                taskForUpdate.LeaderComment = comment;
            else if (executorRole is ProjectMemberRole.Member)
                taskForUpdate.ExecutorComment = comment;
            else
                return false;

            return true;
        }

        public static HITSTask? UpdateTaskStatus(Guid taskId, HITSTaskStatus newStatus, User executor, int taskIndex = -100)
        {
            var taskForUpdate = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (taskForUpdate is null) return null;

            taskForUpdate.Status = newStatus;
            if (taskIndex >= 0)
                taskForUpdate.Position = taskIndex;

            if (newStatus is not HITSTaskStatus.OnModification)
            {
                if (newStatus > HITSTaskStatus.NewTask && taskForUpdate.Executor is null)
                    taskForUpdate.Executor = executor;
                else if (newStatus <= HITSTaskStatus.NewTask)
                    taskForUpdate.Executor = null;
            }

            MockTaskMovementLogs.CreateNewTaskLog(taskForUpdate, taskForUpdate.Initiator, executor);

            return taskForUpdate;
        }

        public static HITSTask? UpdateTaskPosition(Guid taskId, int newIndex)
        {
            var taskForUpdate = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (taskForUpdate is null) return null;

            taskForUpdate.Position = newIndex;

            return taskForUpdate;
        }

        public static bool UpdateTaskPositions(ICollection<HITSTask> tasks)
        {
            foreach (var updatedTask in tasks)
            {
                _tasks.FirstOrDefault(t => t.Id == updatedTask.Id)?.Position = updatedTask.Position;
            }
            return true;
        }

        public static bool DeleteTask(HITSTask task)
        {
            var isRemoved = _tasks.Remove(task);

            if (isRemoved)
                foreach (var sprint in _sprints)
                    sprint.Tasks.Remove(task);

            return isRemoved;
        }
    }
}
