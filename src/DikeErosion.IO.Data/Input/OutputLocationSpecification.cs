namespace DikeErosion.IO.Data.Input;

public class OutputLocationSpecification
{
    public OutputLocationSpecification(double xPosition, double damageStart, string calculationMethod, string topLayerType)
    {
        // TODO: Check for NaN values and throw if they exist.
        XPosition = xPosition;
        CalculationMethod = calculationMethod;
        TopLayerType = topLayerType;
        DamageStart = damageStart;
    }

    public double XPosition { get; }

    public string CalculationMethod { get; }

    // TODO: Convert to enum
    public string TopLayerType { get; }

    // TODO: Convert to enum
    public double DamageStart { get; }

    // TODO: Make derived classes for different type of output locations.
}