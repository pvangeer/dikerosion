using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.Windows.Data;

namespace DikeErosion.Gui;

public class TimeStepsToEndTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<double> timeSteps && timeSteps.Any())
        {
            return timeSteps.Max();
        }

        return 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}