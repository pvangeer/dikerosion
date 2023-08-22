using DikeErosion.Data.CrossShoreProfile;
using OxyPlot;

namespace DikeErosion.Visualization.ViewModels
{
    public static class CharacteristicPointTypeExtensions
    {
        public static string ToTitle(this CharacteristicPointType type)
        {
            switch (type)
            {
                case CharacteristicPointType.CrownBermOuterSlope:
                    return "Kruin berm (buiten)";
                case CharacteristicPointType.CrownOuterSlope:
                    return "Kruin buitenzijde";
                case CharacteristicPointType.InnerPointBermOuterSlope:
                    return "Insteek berm (binnen)";
                case CharacteristicPointType.ToeOuterSlope:
                    return "Teen buitenzijde";
                default:
                    return "Onbekend";
            }
        }

        public static MarkerType ToMarkerType(this CharacteristicPointType type)
        {
            switch (type)
            {
                case CharacteristicPointType.CrownBermOuterSlope:
                    return MarkerType.Diamond;
                case CharacteristicPointType.CrownOuterSlope:
                    return MarkerType.Diamond;
                case CharacteristicPointType.InnerPointBermOuterSlope:
                    return MarkerType.Diamond;
                case CharacteristicPointType.ToeOuterSlope:
                    return MarkerType.Diamond;
                default:
                    return MarkerType.Diamond;
            }
        }

        public static double ToMarkerSize(this CharacteristicPointType type)
        {
            switch (type)
            {
                case CharacteristicPointType.CrownBermOuterSlope:
                    return 5.0;
                case CharacteristicPointType.CrownOuterSlope:
                    return 5.0;
                case CharacteristicPointType.InnerPointBermOuterSlope:
                    return 5.0;
                case CharacteristicPointType.ToeOuterSlope:
                    return 5.0;
                default:
                    return 5.0;
            }
        }

        public static OxyColor ToColor(this CharacteristicPointType type)
        {
            switch (type)
            {
                case CharacteristicPointType.CrownBermOuterSlope:
                    return OxyColors.LightGray;
                case CharacteristicPointType.CrownOuterSlope:
                    return OxyColors.LawnGreen;
                case CharacteristicPointType.InnerPointBermOuterSlope:
                    return OxyColors.GreenYellow;
                case CharacteristicPointType.ToeOuterSlope:
                    return OxyColors.Brown;
                default:
                    return OxyColors.Brown;
            }
        }
    }
}
