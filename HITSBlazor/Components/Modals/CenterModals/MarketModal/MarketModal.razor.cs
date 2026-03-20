using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Notifications;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace HITSBlazor.Components.Modals.CenterModals.MarketModal
{
    public partial class MarketModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Market? Market { get; set; }

        private bool _isLoading = true;
        private bool _submitting = false;

        private string MarketName { get; set; } = string.Empty;
        private string MarketStartDate { get; set; } = string.Empty;
        private string MarketFinishDate { get; set; } = string.Empty;

        private static DateTime? ConvertStringToDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;

            return DateTimeOffset.Parse(
                date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal
            ).UtcDateTime;
        }

        private async Task SendMarket()
        {
            _submitting = true;

            bool isValid = true;

            DateTime? startDate = ConvertStringToDate(MarketStartDate);
            DateTime? finishDate = ConvertStringToDate(MarketFinishDate);

            if (string.IsNullOrWhiteSpace(MarketName)) isValid = false;
            if (startDate.HasValue) isValid = false;
            if (finishDate.HasValue) isValid = false;

            if (!isValid)
            {
                //NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                return;
            }

            bool result;
            if (Market is not null)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                var market = new Market
                {
                    Name = MarketName,
                    StartDate = startDate.Value,
                    FinishDate = finishDate.Value
                };

                //result = await CompanyService.UpdateCompanyAsync(
                //    CompanyId.Value,
                //    CompanyName,
                //    SelectedOwner,
                //    _companyUsers
                //);
                result = true;
            }
            else
            {
                //result = await CompanyService.CreateCompanyAsync(
                //    CompanyName,
                //    SelectedOwner,
                //    _companyUsers
                //);
                result = true;
            }
#pragma warning restore CS8629 // Nullable value type may be null.

            if (result)
                await ModalService.Close(ModalType.Center);

            _submitting = false;
        }
    }
}
