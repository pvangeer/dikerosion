using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DikeErosion.Visualization.DataTemplates;

public class HasTimeStepsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is double[] timeSteps && timeSteps.Any());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}