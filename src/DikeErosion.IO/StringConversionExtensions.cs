using DikeErosion.Data;
using DikeErosion.Data.ExceptionHandling;

namespace DikeErosion.IO
{
    public static class StringConversionExtensions
    {
        private const string NaturalStoneKey = "natuursteen";
        private const string GrassCoverWaveAttackKey = "grasGolfklap";
        private const string GrassCoverWaveRunUpKey = "grasGolfoploop";
        private const string AsphaltCoverWaveAttackKey = "asfaltGolfklap";

        private const string NordicStoneKey = "noorseSteen";
        private const string GrassCoverClosedKey = "grasGeslotenZode";
        private const string WABKey = "waterbouwAsfaltBeton";

        public static CalculationMethod ToCalculationMethod(this string value)
        {
            switch (value)
            {
                case NaturalStoneKey:
                    return CalculationMethod.NaturalStone;
                case GrassCoverWaveAttackKey:
                    return CalculationMethod.GrassCoverWaveAttack;
                case GrassCoverWaveRunUpKey:
                    return CalculationMethod.GrassCoverWaveRunUp;
                case AsphaltCoverWaveAttackKey:
                    return CalculationMethod.AsphaltCoverWaveAttack;
                default:
                    throw new DikeErosionException(DikeErosionExceptionType.UnexpectedCalculationMethod);
            }
        }

        public static TopLayerType ToTopLayerType(this string value)
        {
            switch (value)
            {
                case NordicStoneKey:
                    return TopLayerType.NordicStone;
                case GrassCoverClosedKey:
                    return TopLayerType.GrassCoverClosed;
                case WABKey:
                    return TopLayerType.WAB;
                default:
                    throw new DikeErosionException(DikeErosionExceptionType.UnexpectedTopLayerType);
            }
        }
    }
}


