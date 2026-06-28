namespace HITSBlazor.Components.Modals.CenterModals.EndedSprintModal
{
    public class DatePoint
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public decimal Value { get; set; } = 0;
    }
}
