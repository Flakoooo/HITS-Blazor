using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils.EnumUIConverters;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.SkillModal
{
    public partial class SkillModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public Skill? Skill { get; set; }

        private bool _isLoading = true;
        private bool _submitting = false;

        private readonly List<KeyValuePair<int, string>> _skillTypeOptions =
        [
            new((int)SkillType.Language, EnumUIConverter.GetInfo(SkillType.Language).DisplayText),
            new((int)SkillType.Framework, EnumUIConverter.GetInfo(SkillType.Framework).DisplayText),
            new((int)SkillType.Database, EnumUIConverter.GetInfo(SkillType.Database).DisplayText),
            new((int)SkillType.Devops, EnumUIConverter.GetInfo(SkillType.Devops).DisplayText)
        ];

        private string SkillName { get; set; } = string.Empty;

        private int? _skillType;
        private string SelectedSkillType
        {
            get => _skillType.HasValue ? _skillType.Value.ToString() : string.Empty;
            set
            {
                if (int.TryParse(value, out int intValue))
                    _skillType = intValue;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (Skill is not null)
            {
                SkillName = Skill.Name;
                SelectedSkillType = ((int)Skill.Type).ToString();
            }

            _isLoading = false;
        }

        private async Task SendSkill()
        {
            _submitting = true;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(SkillName)) isValid = false;
            if (!_skillType.HasValue) isValid = false;

            if (!isValid)
            {
                NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                return;
            }

            bool result;
#pragma warning disable CS8629 // Nullable value type may be null.
            if (Skill is not null)
            {
                result = await SkillService.UpdateSkillAsync(new UpdateSkillRequest 
                {
                    Id = Skill.Id, 
                    Name = SkillName, 
                    Type = (SkillType)_skillType.Value, 
                    Confirmed = Skill.Confirmed 
                });
            }
            else
            {
                result = (await SkillService.CreateNewSkillAsync(
                    SkillName,
                    (SkillType)_skillType.Value,
                    true
                )) is not null;
            }
#pragma warning restore CS8629 // Nullable value type may be null.

            if (result)
                await ModalService.Close(ModalType.Center);

            _submitting = false;
        }
    }
}
