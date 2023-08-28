using DikeErosion.Data;
using DikeErosion.Data.ExceptionHandling;

namespace DikeErosion.IO;

public static class StringConversionExtensions
{
    private const string NaturalStoneKey = "natuursteen";
    private const string GrassCoverWaveAttackKey = "grasGolfklap";
    private const string GrassCoverWaveRunUpKey = "grasGolfoploop";
    private const string AsphaltCoverWaveAttackKey = "asfaltGolfklap";

    private const string NordicStoneKey = "noorseSteen";
    private const string GrassCoverClosedKey = "grasGeslotenZode";
    private const string WABKey = "waterbouwAsfaltBeton";
    private const string GrassOvertoppingKey = "grasGolfoverslag";

    public static CalculationMethod ToCalculationMethod(this string value)
    {
        return value switch
        {
            NaturalStoneKey => CalculationMethod.NaturalStone,
            GrassCoverWaveAttackKey => CalculationMethod.GrassCoverWaveAttack,
            GrassCoverWaveRunUpKey => CalculationMethod.GrassCoverWaveRunUp,
            AsphaltCoverWaveAttackKey => CalculationMethod.AsphaltCoverWaveAttack,
            GrassOvertoppingKey => CalculationMethod.GrassCoverOvertopping,
            _ => throw new DikeErosionException(DikeErosionExceptionType.UnexpectedCalculationMethod)
        };
    }

    public static TopLayerType ToTopLayerType(this string value)
    {
        return value switch
        {
            NordicStoneKey => TopLayerType.NordicStone,
            GrassCoverClosedKey => TopLayerType.GrassCoverClosed,
            WABKey => TopLayerType.WAB,
            _ => throw new DikeErosionException(DikeErosionExceptionType.UnexpectedTopLayerType)
        };
    }
}