using DikeErosion.Data;
using DikeErosion.Data.CrossShoreProfile;
using DikeErosion.Data.ExceptionHandling;

namespace DikeErosion.IO;

public class DikeErosionInputImporter
{
    private readonly DikeErosionProject project;

    public DikeErosionInputImporter(DikeErosionProject project)
    {
        this.project = project;
    }

    public void Import(string fileName)
    {
        ValidateFileName(fileName);
        var input = DikeErosionJsonReader.ReadInput(fileName);

        project.InputFileName = fileName;

        project.Profile.Coordinates.Clear();
        foreach (var coordinate in input.Profile.Coordinates)
            project.Profile.Coordinates.Add((CrossShoreCoordinate)coordinate.Clone());

        project.Profile.CharacteristicPoints.Clear();
        AddCharacteristicPoint(input.Profile.ToeOuterSlope, CharacteristicPointType.ToeOuterSlope);
        AddCharacteristicPoint(input.Profile.BermCrownOuterSlope, CharacteristicPointType.CrownBermOuterSlope);
        AddCharacteristicPoint(input.Profile.InsetBermOuterSlope, CharacteristicPointType.InnerPointBermOuterSlope);
        AddCharacteristicPoint(input.Profile.CrownOuterSlope, CharacteristicPointType.CrownOuterSlope);
        AddCharacteristicPoint(input.Profile.CrownInnerSlope, CharacteristicPointType.CrownInnerSlope);
        AddCharacteristicPoint(input.Profile.ToeInnerSlope, CharacteristicPointType.ToeInnerSlope);

        project.TimeSteps.Clear();
        foreach (var timeStep in input.TimeSteps)
            project.TimeSteps.Add(timeStep);

        project.OutputLocations.Clear();
        foreach (var location in input.OutputLocations)
        {
            var zPosition = project.Profile.InterpolateZ(location.XPosition);
            if (!double.IsNaN(zPosition))
                project.OutputLocations.Add(
                    new OutputLocation(new CrossShoreCoordinate(location.XPosition, zPosition),
                        location.CalculationMethod.ToCalculationMethod(),
                        location.TopLayerType.ToTopLayerType(),
                        location.DamageStart));
        }

        project.HydraulicConditions.Clear();
        var timeSteps = input.TimeSteps.ToArray();
        var waterLevels = input.HydraulicConditions.FirstOrDefault(hc => hc.Name == "waterstanden")?.Values;
        var waveHeights = input.HydraulicConditions.FirstOrDefault(hc => hc.Name == "golfhoogtenHm0")?.Values;
        var wavePeriods = input.HydraulicConditions.FirstOrDefault(hc => hc.Name == "golfperiodenTm10")?.Values;
        var waveAngles = input.HydraulicConditions.FirstOrDefault(hc => hc.Name == "golfhoeken")?.Values;
        if (waterLevels != null && waveHeights != null && wavePeriods != null && waveAngles != null &&
            timeSteps.Length - 1 == waterLevels.Length && timeSteps.Length - 1 == wavePeriods.Length &&
            timeSteps.Length - 1 == waveAngles.Length)
            for (var i = 0; i < waterLevels.Length; i++)
                project.HydraulicConditions.Add(new HydraulicCondition
                {
                    TimeStep = timeSteps[i + 1],
                    WaterLevel = waterLevels[i],
                    WaveHeight = waveHeights[i],
                    WavePeriod = wavePeriods[i],
                    WaveAngle = waveAngles[i]
                });

        project.OnPropertyChanged(nameof(DikeErosionProject.InputFileName));
        project.OnPropertyChanged(nameof(DikeErosionProject.Profile));

        DikeErosionProjectHandler.ClearOutput(project);
    }

    private void AddCharacteristicPoint(CrossShoreCoordinate? coordinate, CharacteristicPointType type)
    {
        if (coordinate == null)
            return;
        project.Profile.CharacteristicPoints.Add(new CharacteristicPoint(coordinate.X, coordinate.Z, type));
    }

    private static void ValidateFileName(string fileName)
    {
        if (!File.Exists(fileName))
            throw new DikeErosionException(DikeErosionExceptionType.FileNotFound);
    }
}