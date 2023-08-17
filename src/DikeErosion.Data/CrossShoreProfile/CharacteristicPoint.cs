namespace DikeErosion.Data.CrossShoreProfile;

public class CharacteristicPoint : CrossShoreCoordinate
{
    public CharacteristicPoint(double x, double z, CharacteristicPointType type) : base(x, z)
    {
        Type = type;
    }

    public CharacteristicPointType Type { get; }
}