using ApexCharts;
using HITSBlazor.Models.Common.Entities;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.Components.RightSideModalInfo
{
    public partial class RightSideModalInfo
    {
        [Parameter]
        public bool IsSplitted { get; set; } = false;

        [Parameter]
        public List<RightSideModalInfoItem> Items { get; set; } = [];

        [Parameter]
        public List<Skill> Skills1 { get; set; } = [];

        [Parameter]
        public string Skills1Label { get; set; } = string.Empty;

        [Parameter]
        public List<Skill> Skills2 { get; set; } = [];

        [Parameter]
        public string Skills2Label { get; set; } = string.Empty;

        [Parameter]
        public string SearhButtonTest { get; set; } = string.Empty;

        private RightSideModalCategory _activeInfoCategory = RightSideModalCategory.Info;

        private string GetInfoCategoryClass(RightSideModalCategory category)
                => _activeInfoCategory == category ? "btn-primary" : "btn-secondary";

        private string GetSideClass(bool isSplitted)
        {
            return isSplitted ? "rounded-3 bg-white" : string.Empty;
        }

        private static ApexChartOptions<Skill> GetRadarChartOptions() => new()
        {
            Chart = new Chart
            {
                Type = ChartType.Radar,
                Toolbar = new Toolbar { Show = false }
            },
            Yaxis =
                [
                    new YAxis
                {
                    Min = 0,
                    Max = 1,
                    Labels = new YAxisLabels { Show = false },
                    Show = false
                }
                ],
            Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Style = new AxisLabelStyle
                    {
                        Colors = new List<string> { "#a8a8a8" },
                        FontSize = "11px"
                    }
                }
            },
            Stroke = new Stroke { Width = 2 },
            Fill = new Fill
            {
                Opacity = 0.5,
                Type = new List<FillType> { FillType.Solid }
            },
            Markers = new Markers { Size = 4 },
            Legend = new Legend
            {
                Show = true,
                Position = LegendPosition.Bottom
            }
        };
    }
}
