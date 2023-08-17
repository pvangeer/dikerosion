using System.Windows;
using System.Windows.Controls;
using DikeErosion.Visualization.ViewModels;

namespace DikeErosion.Gui;

public class MainContentTemplateSelector : DataTemplateSelector
{
    public DataTemplate? CrossShoreViewDataTemplate { get; set; }

    public DataTemplate? TimeLineViewDataTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        switch (item)
        {
            case CrossShoreChartViewModel _:
                return CrossShoreViewDataTemplate;
            case TimeLineViewModel _:
                return TimeLineViewDataTemplate;
            default:
                return base.SelectTemplate(item, container);
        }
    }
}