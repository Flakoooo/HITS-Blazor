using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Validation;
using Microsoft.AspNetCore.Components;
using System.Globalization;

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
        public Market? Market { get; set; }

        private bool _isLoading = true;
        private bool _submitting = false;
        private bool _submitted = false;

        private string MarketName { get; set; } = string.Empty;
        private string MarketStartDate { get; set; } = string.Empty;
        private string MarketFinishDate { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (Market is not null)
            {
                MarketName = Market.Name;
                MarketStartDate = Market.StartDate.ToString("yyyy-MM-dd");
                MarketFinishDate = Market.FinishDate.ToString("yyyy-MM-dd");
            }

            _isLoading = false;
        }

        private static DateTime? ConvertStringToDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;

            return DateTimeOffset.Parse(
                date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal
            ).UtcDateTime;
        }

        private ValidationEvaluation ValidFinishDate(string finishDate)
            => MarketValidation.FinishDateValidation(finishDate, MarketStartDate);

        private async Task SendMarket()
        {
            _submitting = true;
            _submitted = false;

            bool isValid = true;

            DateTime? startDate = ConvertStringToDate(MarketStartDate);
            DateTime? finishDate = ConvertStringToDate(MarketFinishDate);

            if (string.IsNullOrWhiteSpace(MarketName)) isValid = false;
            else if (!startDate.HasValue || !MarketValidation.StartDateValidation(startDate.Value).IsValid) isValid = false;
            else if (!finishDate.HasValue || !MarketValidation.FinishDateValidation(finishDate.Value, startDate.Value).IsValid) isValid = false;

            if (!isValid)
            {
                NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                _submitted = true;
                return;
            }

            bool result;
            if (Market is not null)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                result = await MarketService.UpdateMarketAsync(
                    Market.Id, 
                    MarketName, 
                    startDate.Value, 
                    finishDate.Value, 
                    Market.Status
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

            _submitted = true;
            _submitting = false;
        }
    }
}
