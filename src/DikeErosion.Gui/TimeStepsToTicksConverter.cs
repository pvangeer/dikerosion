using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DikeErosion.Gui;

public class TimeStepsToTicksConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ticks = "0";
        if (value is IList<double> timeSteps && timeSteps.Any())
        {
            foreach (var timeStep in timeSteps)
            {
                ticks += (timeStep.ToString(CultureInfo.InvariantCulture) + ",");
            }
            ticks = ticks.Remove(ticks.Length - 1, 1);
        }
        return ticks;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}