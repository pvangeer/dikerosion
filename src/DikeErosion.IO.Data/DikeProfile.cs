using DikeErosion.Data;
using DikeErosion.Data.CrossShoreProfile;

namespace DikeErosion.IO.Data;

public class DikeProfile
{
    public DikeProfile(IEnumerable<CrossShoreCoordinate> coordinates, CrossShoreCoordinate? toeOuterSlope = null, CrossShoreCoordinate? bermCrownOuterSlope = null, CrossShoreCoordinate? insetBermOuterSlope = null, CrossShoreCoordinate? crownOuterSlope = null)
    {
        Coordinates = coordinates;
        CrownOuterSlope = crownOuterSlope;
        InsetBermOuterSlope = insetBermOuterSlope;
        BermCrownOuterSlope = bermCrownOuterSlope;
        ToeOuterSlope = toeOuterSlope;
    }

    public IEnumerable<CrossShoreCoordinate> Coordinates { get; }

    public CrossShoreCoordinate? ToeOuterSlope { get; }

    public CrossShoreCoordinate? BermCrownOuterSlope { get; }

    public CrossShoreCoordinate? InsetBermOuterSlope { get; }

    public CrossShoreCoordinate? CrownOuterSlope { get; }
}