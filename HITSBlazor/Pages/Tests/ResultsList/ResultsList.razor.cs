using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Services.TestResults;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Tests.ResultsList
{
    [Authorize]
    [Route("tests/results")]
    public partial class ResultsList
    {
        [Inject]
        private ITestResultService TestResultService { get; set; } = null!;

        private bool _isLoading = true;

        private string _searchText = string.Empty;

        private readonly List<TableHeaderItem> _resultTableHeader = 
        [
            new() { Text = "Почта"                                                  },
            new() { Text = "Имя"                                                    },
            new() { Text = "Фамилия"                                                },
            new() { Text = "Группа"                                                 },
            new() { Text = "Тест Белбина",                  ColumnClass = "col-2"   },
            new() { Text = "Стиль мышления",                ColumnClass = "col-2"   },
            new() { Text = "Личностный опросник Айзенка",   ColumnClass = "col-3"   }
        ];

        private List<TestAllResponse> _results = [];

        private List<ValueViewModel<string?>> _studyFilterValues = [];
        private HashSet<ValueViewModel<string?>> SelectedStudyGroups { get; set; } = [];

        private readonly List<ValueViewModel<string?>> _belbinResultFilterValues = 
        [
            new("РЕАЛИЗАТОР",       "РЕАЛИЗАТОР"        ),
            new("КООРДИНАТОР",      "КООРДИНАТОР"       ),
            new("МОТИВАТОР",        "МОТИВАТОР"         ),
            new("ГЕНЕРАТОР ИДЕЙ",   "ГЕНЕРАТОР ИДЕЙ"    ),
            new("ИССЛЕДОВАТЕЛЬ",    "ИССЛЕДОВАТЕЛЬ"     ),
            new("АНАЛИТИК-ЭКСПЕРТ", "АНАЛИТИК-ЭКСПЕРТ"  ),
            new("ВДОХНОВИТЕЛЬ",     "ВДОХНОВИТЕЛЬ"      ),
            new("КОНТРОЛЕР",        "КОНТРОЛЕР"         ),
            new("СПЕЦИАЛИСТ",       "СПЕЦИАЛИСТ"        )
        ];
        private HashSet<ValueViewModel<string?>> SelectedBelbinResult { get; set; } = [];

        private readonly List<ValueViewModel<string?>> _mindResultFilterValues =
        [
            new("Все",              "Все"               ),
            new("Синтетический",    "Синтетический"     ),
            new("Идеалистический",  "Идеалистический"   ),
            new("Прагматический",   "Прагматический"    ),
            new("Аналитический",    "Аналитический"     ),
            new("Реалистический",   "Реалистический"    )
        ];
        private ValueViewModel<string?>? SelectedMindResult { get; set; }

        private readonly List<ValueViewModel<string?>> _temperExtraversionFilterValues =
        [
            new("Яркий экстраверт",     "Яркий экстраверт"  ),
            new("Экстраверт",           "Экстраверт"        ),
            new("Среднее значение",     "Среднее значение"  ),
            new("Интроверт",            "Интроверт"         ),
            new("Глубокий интроверт",   "Глубокий интроверт")
        ];
        private HashSet<ValueViewModel<string?>> SelectedTemperExtraversion { get; set; } = [];

        private readonly List<ValueViewModel<string?>> _temperNeurotismFilterValues =
        [
            new("Очень высокий уровень нейротизма", "Очень высокий уровень нейротизма"  ),
            new("Высокий уровень нейротизма",       "Высокий уровень нейротизма"        ),
            new("Среднее значение",                 "Среднее значение"                  ),
            new("Низкий уровень нейротизма",        "Низкий уровень нейротизма"         )
        ];
        private HashSet<ValueViewModel<string?>> SelectedTemperNeurotism { get; set; } = [];

        private readonly List<ValueViewModel<string?>> _temperLieFilterValues =
        [
            new("Неискренность в ответах",  "Неискренность в ответах"   ),
            new("Норма",                    "Норма"                     )
        ];
        private HashSet<ValueViewModel<string?>> SelectedTemperLie { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTestsResultsAsync();
            _studyFilterValues = [.. _results.Select(r => r.User.StudyGroup).Distinct().Select(sg => new ValueViewModel<string?>(sg, sg))];

            //foreach( var result in _results)
            //{
            //    Console.WriteLine(result.BelbinResult);
            //    Console.WriteLine(result.TemperResult);
            //    Console.WriteLine(result.MindResult);
            //}

            _isLoading = false;
        }

        private async Task LoadTestsResultsAsync()
        {
            _results = await TestResultService.GetTestsResultsAsync(
                searchText: _searchText
            );
        }

        private async Task SearchResult(string value)
        {
            _searchText = value;
            await LoadTestsResultsAsync();
        }

        private async Task ResetFilters()
        {
            await LoadTestsResultsAsync();
        }
    }
}
