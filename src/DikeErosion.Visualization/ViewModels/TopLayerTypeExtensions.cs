using DikeErosion.Data;
using OxyPlot;

namespace DikeErosion.Visualization.ViewModels
{
    public static class TopLayerTypeExtensions
    {
        public static string ToTitle(this TopLayerType type)
        {
            switch (type)
            {
                case TopLayerType.WAB:
                    return "Waterbouw asfaltbeton";
                case TopLayerType.NordicStone:
                    return "Noorse steen";
                case TopLayerType.GrassCoverClosed:
                    return "Gras - gesloten";
                default:
                    return type.ToString();
            }
        }

        public static OxyColor ToStrokeColor(this TopLayerType type)
        {
            switch (type)
            {
                case TopLayerType.WAB:
                    return OxyColors.DarkGray;
                case TopLayerType.NordicStone:
                    return OxyColors.Chocolate;
                case TopLayerType.GrassCoverClosed:
                    return OxyColors.DarkGreen;
                default:
                    return OxyColors.DarkBlue;
            }
        }
    }
}
