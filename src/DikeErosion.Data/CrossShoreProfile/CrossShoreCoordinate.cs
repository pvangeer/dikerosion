using DikeErosion.Data.ExceptionHandling;

namespace DikeErosion.Data.CrossShoreProfile;

public class CrossShoreCoordinate : ICloneable
{
    public CrossShoreCoordinate(double x, double z)
    {
        if (double.IsNaN(x) || double.IsInfinity(x))
            throw new DikeErosionException(DikeErosionExceptionType.XShouldNotBeValidValue);
        if (double.IsNaN(z) || double.IsInfinity(z))
            throw new DikeErosionException(DikeErosionExceptionType.ZShouldNotBeValidValue);

        X = x;
        Z = z;
    }

    public double X { get; }

    public double Z { get; }

    public object Clone()
    {
        return new CrossShoreCoordinate(X, Z);
    }
}