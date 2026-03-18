using System.ComponentModel;

namespace HITSBlazor.Components.ActionMenus.BaseActionMenu
{
    public enum MenuAction
    {
        [Description("Просмотреть")]
        View,

        [Description("Открыть идею")]
        ViewIdea,

        [Description("Открыть идею")]
        ViewIdeaMarket,

        [Description("Перейти на профиль")]
        ViewProfile,

        [Description("Просмотреть письмо")]
        ViewLetter,

        [Description("Редактировать")]
        Edit,

        [Description("Утвердить")]
        Confirm,

        [Description("Принять")]
        TeamRequestAccept,

        [Description("Назначить лидером")]
        SetLeader,

        [Description("Удалить")]
        Delete,

        [Description("Отклонить")]
        TeamRequestCancel,

        [Description("Снять роль лидера")]
        UnsetLeader,

        [Description("Исключить")]
        RemoveTeamMember
    }
}
