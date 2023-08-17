using DikeErosion.Data.CrossShoreProfile;

namespace DikeErosion.Data
{
    public class OutputLocation
    {
        public OutputLocation(CrossShoreCoordinate coordinate, CalculationMethod calculationMethod = CalculationMethod.GrassCoverWaveAttack, TopLayerType topLayerType = TopLayerType.GrassCoverClosed, double damageStart = 0.0)
        {
            Coordinate = coordinate;
            TopLayerType = topLayerType;
            DamageStart = damageStart;
            CalculationMethod = calculationMethod;
        }

        public CrossShoreCoordinate Coordinate { get; set; }

        public CalculationMethod CalculationMethod { get; set; }

        public TopLayerType TopLayerType { get; set; }

        public double DamageStart { get; set; }
    }
}
