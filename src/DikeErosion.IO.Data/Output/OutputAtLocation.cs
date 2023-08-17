namespace DikeErosion.IO.Data.Output;

public class OutputAtLocation
{
    public OutputAtLocation(double height, double outerSlope, bool revetmentFailed, double revetmentFailedAfter, double[] damageDevelopment)
    {
        PhysicsDouble = new Dictionary<string, double[]>();
        PhysicsBool = new Dictionary<string, bool[]>();
        Height = height;
        OuterSlope = outerSlope;
        RevetmentFailed = revetmentFailed;
        RevetmentFailedAfter = revetmentFailedAfter;
        DamageDevelopment = damageDevelopment;
    }

    public double Height { get; }

    public double OuterSlope { get; }

    public bool RevetmentFailed { get; }

    /// <summary>
    /// Moment of failure in seconds
    /// </summary>
    public double RevetmentFailedAfter { get; }

    // TODO: Add output timesteps (and other relevant information)?
    // public double[] OutputTimeSteps { get; }

    public double[] DamageDevelopment { get; }

    // TODO: Create typed object for results?
    public Dictionary<string, double[]> PhysicsDouble { get; }

    public Dictionary<string, bool[]> PhysicsBool { get; }
}