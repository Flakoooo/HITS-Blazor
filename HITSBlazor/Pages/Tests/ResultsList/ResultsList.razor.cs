using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Tests.Entities;

namespace HITSBlazor.Pages.Tests.ResultsList
{
    public partial class ResultsList
    {
        private readonly List<TableHeaderItem> _resultTableHeader = 
        [
            new() { Text = "Почта" },
            new() { Text = "Имя" },
            new() { Text = "Фамилия" },
            new() { Text = "Группа" },
            new() { Text = "Тест Белбина" },
            new() { Text = "Стиль мышления" },
            new() { Text = "Личностный опросник Айзенка" }
        ];

        private List<TestAllResponse> _results = [];

        private void SearchResult(string value)
        {

        }
    }
}
