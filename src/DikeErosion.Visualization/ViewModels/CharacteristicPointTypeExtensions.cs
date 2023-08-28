using DikeErosion.Data.CrossShoreProfile;
using OxyPlot;

namespace DikeErosion.Visualization.ViewModels;

public static class CharacteristicPointTypeExtensions
{
    public static string ToTitle(this CharacteristicPointType type)
    {
        return type switch
        {
            CharacteristicPointType.CrownBermOuterSlope => "Kruin berm (buiten)",
            CharacteristicPointType.CrownOuterSlope => "Kruin buitenzijde",
            CharacteristicPointType.InnerPointBermOuterSlope => "Insteek berm (binnen)",
            CharacteristicPointType.ToeOuterSlope => "Teen buitenzijde",
            CharacteristicPointType.ToeInnerSlope => "Teen binnenzijde",
            CharacteristicPointType.CrownInnerSlope => "Kruin binnenzijde",
            _ => "Onbekend"
        };
    }

    public static MarkerType ToMarkerType(this CharacteristicPointType type)
    {
        return type switch
        {
            CharacteristicPointType.CrownBermOuterSlope => MarkerType.Diamond,
            CharacteristicPointType.CrownOuterSlope => MarkerType.Diamond,
            CharacteristicPointType.InnerPointBermOuterSlope => MarkerType.Diamond,
            CharacteristicPointType.ToeOuterSlope => MarkerType.Diamond,
            CharacteristicPointType.CrownInnerSlope => MarkerType.Diamond,
            CharacteristicPointType.ToeInnerSlope => MarkerType.Diamond,
            _ => MarkerType.Diamond
        };
    }

    public static double ToMarkerSize(this CharacteristicPointType type)
    {
        return type switch
        {
            CharacteristicPointType.CrownBermOuterSlope => 5.0,
            CharacteristicPointType.CrownOuterSlope => 5.0,
            CharacteristicPointType.InnerPointBermOuterSlope => 5.0,
            CharacteristicPointType.ToeOuterSlope => 5.0,
            CharacteristicPointType.ToeInnerSlope => 5.0,
            CharacteristicPointType.CrownInnerSlope => 5.0,
            _ => 5.0
        };
    }

    public static OxyColor ToColor(this CharacteristicPointType type)
    {
        return type switch
        {
            CharacteristicPointType.CrownBermOuterSlope => OxyColors.LightGray,
            CharacteristicPointType.CrownOuterSlope => OxyColors.LawnGreen,
            CharacteristicPointType.InnerPointBermOuterSlope => OxyColors.GreenYellow,
            CharacteristicPointType.ToeOuterSlope => OxyColors.Brown,
            CharacteristicPointType.ToeInnerSlope => OxyColors.DarkGreen,
            CharacteristicPointType.CrownInnerSlope => OxyColors.DarkSeaGreen,
            _ => OxyColors.Brown
        };
    }
}