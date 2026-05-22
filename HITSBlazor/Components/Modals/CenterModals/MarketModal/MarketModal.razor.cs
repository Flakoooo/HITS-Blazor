using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Validation;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.MarketModal
{
    public partial class MarketModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Parameter]
        public Guid? MarketId { get; set; }

        private bool _isLoading = true;
        private bool _submitting = false;

        private Dictionary<string, string> _errors = [];

        private string MarketName { get; set; } = string.Empty;
        private string MarketStartDate { get; set; } = string.Empty;
        private string MarketFinishDate { get; set; } = string.Empty;

        private MarketStatus? _currentMarketStatus = null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (MarketId.HasValue)
            {
                var market = await MarketService.GetMarketByIdAsync(MarketId.Value);
                if (market is null) return;

                MarketName = market.Name;
                MarketStartDate = market.StartDate.ToString("yyyy-MM-dd");
                MarketFinishDate = market.FinishDate.ToString("yyyy-MM-dd");
                _currentMarketStatus = market.Status;
            }

            _isLoading = false;
        }

        private async Task SendMarket()
        {
            _errors.Clear();

            _submitting = true;

            if (string.IsNullOrWhiteSpace(MarketName))
                _errors.Add("name", "Поле не может быть пустым");

            DateOnly? startDate = null;
            var validationStartDateResult = DateValidation.StartDateValidation(MarketStartDate, ref startDate);
            if (!validationStartDateResult.IsValid)
                _errors.Add("startDate", validationStartDateResult.Message);

            DateOnly? finishDate = null;
            var validationFinishDateResult = DateValidation.FinishDateValidation(MarketFinishDate, MarketStartDate, ref finishDate);
            if (!validationFinishDateResult.IsValid)
                _errors.Add("finishDate", validationFinishDateResult.Message);

            if (_errors.Count > 0)
            {
                NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                return;
            }

            bool result;
            if (MarketId.HasValue)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                result = await MarketService.UpdateMarketAsync(
                    MarketId.Value, 
                    MarketName, 
                    startDate.Value, 
                    finishDate.Value,
                    _currentMarketStatus.Value
                );
            }
            else
            {
                result = await MarketService.CreateNewMarketAsync(
                    MarketName,
                    startDate.Value,
                    finishDate.Value
                );
            }
#pragma warning restore CS8629 // Nullable value type may be null.

            if (result)
                await ModalService.Close(ModalType.Center);

            _submitting = false;
        }
    }
}
