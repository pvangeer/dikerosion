namespace DikeErosion.Data;

public class LocationSpecificOutput
{
    public LocationSpecificOutput(OutputLocation location, bool failed, double failureTimeStep, double calculatedOuterSlope)
    {
        Location = location;
        Failed = failed;
        FailureTimeStep = failureTimeStep;
        CalculatedOuterSlope = calculatedOuterSlope;
    }

    public OutputLocation Location { get; }

    public bool Failed { get; }

    public double FailureTimeStep { get; }

    public double CalculatedOuterSlope { get; }
}