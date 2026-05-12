namespace HITSBlazor.Models.Common.Responses
{
    public record class ListDataResponse<T>(int Count, IReadOnlyCollection<T> List);
}
