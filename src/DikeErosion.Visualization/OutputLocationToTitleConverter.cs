using System;
using System.Globalization;
using System.Windows.Data;
using DikeErosion.Data;
using DikeErosion.Visualization.ViewModels;

namespace DikeErosion.Visualization;

public class OutputLocationToTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is OutputLocation location)
        {
            return $"X = {location.Coordinate.X} m  (Z = {location.Coordinate.Z:F2}) - {location.TopLayerType.ToTitle()}";
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}