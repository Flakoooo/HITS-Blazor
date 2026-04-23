using HITSBlazor.Components.ActionMenus.BaseActionMenu;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetActionMenuInfo(MenuAction action) => action switch
        {
            MenuAction.View => new EnumUIResult("Просмотреть", string.Empty),
            MenuAction.Edit => new EnumUIResult("Редактировать", string.Empty),
            MenuAction.ViewProfile => new EnumUIResult("Перейти на профиль", string.Empty),
            MenuAction.ViewIdea => new EnumUIResult("Открыть идею", string.Empty),
            MenuAction.ViewIdeaMarket => new EnumUIResult("Открыть идею", string.Empty), //TODO: может убрать?
            MenuAction.ViewLetter => new EnumUIResult("Просмотреть письмо", string.Empty),
            MenuAction.Confirm => new EnumUIResult("Утвердить", string.Empty),
            MenuAction.ViewTeamProfile => new EnumUIResult("Профиль команды", string.Empty),
            MenuAction.ViewProject => new EnumUIResult("Перейти в проект", string.Empty),
            MenuAction.TeamCreateSelectMember => new EnumUIResult("Выбрать", "text-success"),
            MenuAction.TeamRequestAccept => new EnumUIResult("Принять", "text-success"),
            MenuAction.StartMarket => new EnumUIResult("Запустить", "text-success"),
            MenuAction.SetLeader => new EnumUIResult("Назначить лидером", "text-primary"),
            MenuAction.GoToMarket => new EnumUIResult("Перейти на биржу", "text-primary"),
            MenuAction.Delete => new EnumUIResult("Удалить", "text-danger"),
            MenuAction.TeamCreateRemoveMember => new EnumUIResult("Отменить выбор", "text-danger"),
            MenuAction.TeamRequestCancel => new EnumUIResult("Отклонить", "text-danger"),
            MenuAction.UnsetLeader => new EnumUIResult("Снять роль лидера", "text-danger"),
            MenuAction.RemoveTeamMember => new EnumUIResult("Исключить", "text-danger"),
            MenuAction.FinishMarket => new EnumUIResult("Завершить", "text-danger"),
            _ => new EnumUIResult(action.ToString(), string.Empty)
        };
    }
}
