using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.EnumTranslators;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Components.Modals.CenterModals.UsersGroupModal
{
    public class RoleTypeValues : ViewModelBase
    {
        public RoleType Value { get; set; }

        public override string GetDisplayInfo() => Value.GetTranslation();
        public override object GetId() => Value;
    }
}
