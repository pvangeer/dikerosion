using System;
using System.Globalization;
using System.Windows.Data;
using DikeErosion.Data;

namespace DikeErosion.Visualization;

public class OutputVariableToTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeDependentOutputVariable output)
        {
            return output.Name;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}