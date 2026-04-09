using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace HITSBlazor.Components.Modals.CenterModals.SprintModal
{
    public partial class SprintModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private string Name { get; set; } = string.Empty;
        private string Goal { get; set; } = string.Empty;
        //MarketStartDate = Market.StartDate.ToString("yyyy-MM-dd");
        private string StartDate { get; set; } = string.Empty;
        private string FinishDate { get; set; } = string.Empty;

        private List<Models.Projects.Entities.Task> _tasks = [];

        private static DateTime? ConvertStringToDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;

            return DateTimeOffset.Parse(
                date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal
            ).UtcDateTime;
        }
    }
}
