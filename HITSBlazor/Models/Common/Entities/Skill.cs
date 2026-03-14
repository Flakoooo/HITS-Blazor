using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Common.Entities
{
    public class Skill : ViewModelBase, IEquatable<Skill>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SkillType Type { get; set; }
        public bool Confirmed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? UpdaterId { get; set; }
        public Guid? DeleterId { get; set; }

        public bool Equals(Skill? other) => other is not null && Id.Equals(other.Id);

        public override bool Equals(object? obj) => Equals(obj as Skill);

        public override string GetDisplayInfo() => Name;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
