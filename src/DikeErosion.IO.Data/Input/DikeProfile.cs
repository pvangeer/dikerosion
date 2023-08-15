namespace DikeErosion.IO.Data.Input;

public class DikeProfile
{
    public DikeProfile(IEnumerable<ProfileCoordinate> coordinates, ProfileCoordinate? toeOuterSlope = null, ProfileCoordinate? bermCrownOuterSlope = null, ProfileCoordinate? insetBermOuterSlope = null, ProfileCoordinate? crownOuterSlope = null)
    {
        Coordinates = coordinates;
        CrownOuterSlope = crownOuterSlope;
        InsetBermOuterSlope = insetBermOuterSlope;
        BermCrownOuterSlope = bermCrownOuterSlope;
        ToeOuterSlope = toeOuterSlope;
    }

    public IEnumerable<ProfileCoordinate> Coordinates { get; }

    public ProfileCoordinate? ToeOuterSlope { get; }

    public ProfileCoordinate? BermCrownOuterSlope { get; }

    public ProfileCoordinate? InsetBermOuterSlope { get; }

    public ProfileCoordinate? CrownOuterSlope { get; }
}