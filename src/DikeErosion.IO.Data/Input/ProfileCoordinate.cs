namespace DikeErosion.IO.Data.Input;

public class ProfileCoordinate
{
    public ProfileCoordinate(double x, double z)
    {
        X = x;
        Z = z;
    }

    public double X { get; }

    public double Z { get; }
}