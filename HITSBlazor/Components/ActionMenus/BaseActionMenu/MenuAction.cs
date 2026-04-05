using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Components.ActionMenus.BaseActionMenu
{
    public enum MenuAction
    {
        [Description("Просмотреть")]
        [Style("")]
        View,

        [Description("Редактировать")]
        [Style("")]
        Edit,

        [Description("Перейти на профиль")]
        [Style("")]
        ViewProfile,

        [Description("Открыть идею")]
        [Style("")]
        ViewIdea,

        [Description("Открыть идею")]
        [Style("")]
        ViewIdeaMarket,

        [Description("Просмотреть письмо")]
        [Style("")]
        ViewLetter,

        [Description("Утвердить")]
        [Style("")]
        Confirm,

        [Description("Профиль команды")]
        [Style("")]
        ViewTeamProfile,

        //=== text-success стили ===
        [Description("Принять")]
        [Style("text-success")]
        TeamRequestAccept,

        [Description("Запустить")]
        [Style("text-success")]
        StartMarket,

        //=== text-primary стили ===
        [Description("Назначить лидером")]
        [Style("text-primary")]
        SetLeader,

        [Description("Перейти на биржу")]
        [Style("text-primary")]
        GoToMarket,

        //=== text-danger стили ===
        [Description("Удалить")]
        [Style("text-danger")]
        Delete,

        [Description("Отклонить")]
        [Style("text-danger")]
        TeamRequestCancel,

        [Description("Снять роль лидера")]
        [Style("text-danger")]
        UnsetLeader,

        [Description("Исключить")]
        [Style("text-danger")]
        RemoveTeamMember,

        [Description("Завершить")]
        [Style("text-danger")]
        FinishMarket
    }
}
