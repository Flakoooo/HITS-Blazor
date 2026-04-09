using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Common.Entities
{
    public class Tag : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? UpdaterId { get; set; }
        public Guid? DeleterId { get; set; }

        public override string GetDisplayInfo() => Name;
        public override object GetId() => Id;
    }
}
