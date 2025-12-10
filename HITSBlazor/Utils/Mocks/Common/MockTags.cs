using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockTags
    {
        private static readonly List<Tag> _tags = CreateTags();

        public static string FrontendId { get; } = Guid.NewGuid().ToString();
        public static string BackendId { get; } = Guid.NewGuid().ToString();
        public static string RefactorId { get; } = Guid.NewGuid().ToString();
        public static string LearningId { get; } = Guid.NewGuid().ToString();
        public static string RunId { get; } = Guid.NewGuid().ToString();
        public static string UIUXId { get; } = Guid.NewGuid().ToString();
        public static string NotificationId { get; } = Guid.NewGuid().ToString();
        public static string IntegrationId { get; } = Guid.NewGuid().ToString();
        public static string OptimizationId { get; } = Guid.NewGuid().ToString();
        public static string StatisticId { get; } = Guid.NewGuid().ToString();
        public static string SecurityId { get; } = Guid.NewGuid().ToString();
        public static string DesignId { get; } = Guid.NewGuid().ToString();

        private static List<Tag> CreateTags() => [
            new Tag { Id = FrontendId,      Name = "Фронтенд",      Color = "#0D6EFD",  Confirmed = true    },
            new Tag { Id = BackendId,       Name = "Бекенд",        Color = "#FFA800",  Confirmed = true    },
            new Tag { Id = RefactorId,      Name = "Рефактор",      Color = "#AEA709",  Confirmed = true    },
            new Tag { Id = LearningId,      Name = "Изучения",      Color = "#13C63A",  Confirmed = true    },
            new Tag { Id = RunId,           Name = "Бегать",        Color = "#BB2D3B",  Confirmed = false   },
            new Tag { Id = UIUXId,          Name = "UI/UX",         Color = "#6fa8dc",  Confirmed = true    },
            new Tag { Id = NotificationId,  Name = "Уведомления",   Color = "#6aa84f",  Confirmed = true    },
            new Tag { Id = IntegrationId,   Name = "Интеграция",    Color = "#674ea7",  Confirmed = true    },
            new Tag { Id = OptimizationId,  Name = "Оптимизация",   Color = "#cc0000",  Confirmed = true    },
            new Tag { Id = StatisticId,     Name = "Статистика",    Color = "#76a5af",  Confirmed = true    },
            new Tag { Id = SecurityId,      Name = "Безопасность",  Color = "#660000",  Confirmed = true    },
            new Tag { Id = DesignId,        Name = "Дизайн",        Color = "#8fce00",  Confirmed = true    }
        ];

        public static Tag? GetTagById(string id)
            => _tags.FirstOrDefault(t => t.Id == id);
    }
}
