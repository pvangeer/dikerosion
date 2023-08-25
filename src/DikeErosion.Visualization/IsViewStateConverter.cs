using System;
using System.Globalization;
using System.Windows.Data;
using DikeErosion.Visualization.ViewModels;

namespace DikeErosion.Visualization;

public class IsViewStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is ViewState viewState) || !(parameter is ViewState desiredViewState))
        {
            throw new Exception("Input is of incorrect type");
        }
        return viewState == desiredViewState;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return parameter;
    }
}