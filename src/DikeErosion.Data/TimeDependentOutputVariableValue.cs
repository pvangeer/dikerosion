using DikeErosion.Data.CrossShoreProfile;

namespace DikeErosion.Data;

public class TimeDependentOutputVariableValue
{
    public TimeDependentOutputVariableValue(CrossShoreCoordinate coordinate, double time, object value)
    {
        Coordinate = coordinate;
        Time = time;
        Value = value;
    }

    public CrossShoreCoordinate Coordinate { get; }

    public double Time { get; }

    public object Value { get; }
}