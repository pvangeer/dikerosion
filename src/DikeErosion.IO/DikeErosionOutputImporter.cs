using DikeErosion.Data;
using DikeErosion.Data.CrossShoreProfile;
using DikeErosion.Data.ExceptionHandling;
using DikeErosion.IO.Data.Output;

namespace DikeErosion.IO;

public class DikeErosionOutputImporter
{
    private readonly DikeErosionProject project;

    public DikeErosionOutputImporter(DikeErosionProject project)
    {
        this.project = project;
    }

    public void Import(string fileName)
    {
        if (!project.TimeSteps.Any() || !project.OutputLocations.Any())
            throw new DikeErosionException(DikeErosionExceptionType.SpecifyInputFirst);

        ValidateFileName(fileName);

        var results = DikeErosionJsonReader.ReadOutput(fileName);
        if (!results.Any())
            return;

        var doubleVariableNames = new List<string>();
        var boolVariableNames = new List<string>();
        foreach (var result in results)
        {
            doubleVariableNames.AddRange(result.PhysicsDouble.Keys.Where(k => !doubleVariableNames.Contains(k)));
            boolVariableNames.AddRange(result.PhysicsBool.Keys.Where(k => !boolVariableNames.Contains(k)));
        }

        // TODO: Check correct length of output variables (correct number of time steps)?

        var heights = results.Select(r => r.Height).ToArray();
        if (heights.Length != project.OutputLocations.Count)
            throw new DikeErosionException(DikeErosionExceptionType.OutputDoesNotMatchInput);

        var coordinatesDictionary = new Dictionary<OutputAtLocation, CrossShoreCoordinate>();
        var locationsDictionary = new Dictionary<OutputAtLocation, OutputLocation>();
        for (var i = 0; i < heights.Length; i++)
        {
            coordinatesDictionary[results[i]] = project.OutputLocations[i].Coordinate;
            locationsDictionary[results[i]] = project.OutputLocations[i];
        }

        var outputVariableValues = InitializeOutputVariableValues(doubleVariableNames, results, coordinatesDictionary, boolVariableNames);
        var locationSpecificOutput = new List<LocationSpecificOutput>();
        foreach (var location in results)
        {
            var currentCoordinate = coordinatesDictionary[location];

            for (var i = 1; i < project.TimeSteps.Count; i++)
            {
                if (doubleVariableNames.Any())
                    foreach (var variableName in doubleVariableNames.Where(v => location.PhysicsDouble.ContainsKey(v)))
                        outputVariableValues[variableName].Add(new TimeDependentOutputVariableValue(
                            currentCoordinate,
                            project.TimeSteps[i],
                            location.PhysicsDouble[variableName][i - 1]));

                if (boolVariableNames.Any())
                    foreach (var variableName in boolVariableNames.Where(v => location.PhysicsBool.ContainsKey(v)))
                        outputVariableValues[variableName].Add(new TimeDependentOutputVariableValue(
                            currentCoordinate,
                            project.TimeSteps[i],
                            location.PhysicsBool[variableName][i - 1]));

                if (location.DamageDevelopment != null)
                    outputVariableValues["Schadeontwikkeling"].Add(
                        new TimeDependentOutputVariableValue(
                            currentCoordinate,
                            project.TimeSteps[i],
                            location.DamageDevelopment[i - 1]));
            }

            locationSpecificOutput.Add(new LocationSpecificOutput(locationsDictionary[location], location.RevetmentFailed,
                location.RevetmentFailedAfter, location.OuterSlope));
        }

        var timeDependentOutputVariables = outputVariableValues.Select(v =>
            new TimeDependentOutputVariable(v.Key, v.Value.First().Value.GetType(), v.Value.ToArray()));
        project.TimeDependentOutputVariables.Clear();
        foreach (var variable in timeDependentOutputVariables)
            project.TimeDependentOutputVariables.Add(variable);

        project.LocationSpecificOutputVariables.Clear();
        foreach (var specificOutput in locationSpecificOutput)
            project.LocationSpecificOutputVariables.Add(specificOutput);

        project.OutputFileName = fileName;
        project.OnPropertyChanged(nameof(DikeErosionProject.OutputFileName));
    }

    private Dictionary<string, List<TimeDependentOutputVariableValue>> InitializeOutputVariableValues(List<string> doubleVariableNames,
        OutputAtLocation[] results, Dictionary<OutputAtLocation, CrossShoreCoordinate> coordinatesDictionary,
        List<string> boolVariableNames)
    {
        var outputVariableValues = new Dictionary<string, List<TimeDependentOutputVariableValue>>();
        foreach (var name in doubleVariableNames)
            outputVariableValues[name] = GetDefaultOutputVariableList(results, coordinatesDictionary, double.NaN);
        foreach (var name in boolVariableNames)
            outputVariableValues[name] = GetDefaultOutputVariableList(results, coordinatesDictionary, false);
        outputVariableValues["Schadeontwikkeling"] = GetDefaultOutputVariableList(results, coordinatesDictionary, double.NaN);
        return outputVariableValues;
    }

    private List<TimeDependentOutputVariableValue> GetDefaultOutputVariableList(OutputAtLocation[] results,
        Dictionary<OutputAtLocation, CrossShoreCoordinate> coordinates, object value)
    {
        return results.Select(r => new TimeDependentOutputVariableValue(coordinates[r], project.TimeSteps[0], value)).ToList();
    }

    private static void ValidateFileName(string fileName)
    {
        if (!File.Exists(fileName))
            throw new DikeErosionException(DikeErosionExceptionType.FileNotFound);
    }
}