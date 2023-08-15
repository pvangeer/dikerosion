namespace DikeErosion.IO.Data.Input;

public class CharacteristicPoint : ProfileCoordinate
{
    public CharacteristicPoint(double x, double z) : base(x, z)
    {
    }

    public CharacteristicPointType Type { get; }
}