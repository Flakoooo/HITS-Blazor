using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockTags
    {
        public static Guid FrontendId { get; } = Guid.NewGuid();
        public static Guid BackendId { get; } = Guid.NewGuid();
        public static Guid RefactorId { get; } = Guid.NewGuid();
        public static Guid LearningId { get; } = Guid.NewGuid();
        public static Guid RunId { get; } = Guid.NewGuid();
        public static Guid UIUXId { get; } = Guid.NewGuid();
        public static Guid NotificationId { get; } = Guid.NewGuid();
        public static Guid IntegrationId { get; } = Guid.NewGuid();
        public static Guid OptimizationId { get; } = Guid.NewGuid();
        public static Guid StatisticId { get; } = Guid.NewGuid();
        public static Guid SecurityId { get; } = Guid.NewGuid();
        public static Guid DesignId { get; } = Guid.NewGuid();

        private static readonly List<Tag> _tags = CreateTags();

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

        public static List<Tag> GetTags() => [.. _tags];

        public static Tag? GetTagById(Guid id)
            => _tags.FirstOrDefault(t => t.Id == id);

        public static Tag? CreateTag(string name, string color, bool IsConfirmed, Guid creatorId)
        {
            var newTag = new Tag 
            {
                Name = name,
                Color = color,
                Confirmed = IsConfirmed,
                CreatorId = creatorId
            };

            _tags.Add(newTag);

            return newTag;
        }

        public static bool ConfirmTag(Guid tagId, Guid updatorId)
        {
            var tagForUpdate = _tags.FirstOrDefault(t => t.Id == tagId);
            if (tagForUpdate is null) return false;

            tagForUpdate.Confirmed = true;
            tagForUpdate.UpdaterId = updatorId;

            return true;
        }

        public static Tag? UpdateTag(Guid tagId, string name, string color, Guid updatorId)
        {
            var tagForUpdate = _tags.FirstOrDefault(t => t.Id == tagId);
            if (tagForUpdate is null) return null;

            tagForUpdate.Name = name;
            tagForUpdate.Color = color;
            tagForUpdate.UpdaterId = updatorId;

            return tagForUpdate;
        }

        public static bool DeleteTag(Tag tag) => _tags.Remove(tag);
    }
}
