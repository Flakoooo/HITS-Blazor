namespace HITSBlazor.Models.Common.Responses
{
    public class ListDataResponse<T>
    {
        public int Count { get; set; } = 0;
        public ICollection<T> List { get; set; } = [];
    }
}
