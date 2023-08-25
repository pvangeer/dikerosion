using DikeErosion.Data;
using DikeErosion.Data.ExceptionHandling;
using DikeErosion.IO.Data.Output;

namespace DikeErosion.IO
{
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
            {
                // TODO: Log warning? Most probably input is not yet specified.
                return;
            }

            ValidateFileName(fileName);

            var results = DikeErosionJsonReader.ReadResults(fileName);
            if (!results.Any())
            {
                return;
            }

            var doubleVariableNames = new List<string>();
            var boolVariableNames = new List<string>();
            foreach (var result in results)
            {
                doubleVariableNames.AddRange(result.PhysicsDouble.Keys.Where(k => !doubleVariableNames.Contains(k)));
                boolVariableNames.AddRange(result.PhysicsBool.Keys.Where(k => !boolVariableNames.Contains(k)));
            }

            /*if ((doubleVariableNames.Any() && firstResult.PhysicsDouble.Values.First().Length != project.TimeSteps.Count -1) ||
                (boolPhysicsResultsAvailable && firstResult.PhysicsDouble.Values.First().Length != project.TimeSteps.Count - 1))
            {
                return;
            }*/
            // TODO: Check correct length?

            var heights = results.Select(r => r.Height).ToArray();
            if (heights.Length != project.OutputLocations.Count)
            {
                // TODO: Log error.
                return;
            }
            var coordinates = heights.ToDictionary(h => h, h => project.OutputLocations.FirstOrDefault(l => Math.Abs(l.Coordinate.Z - h) < 1E-8));
            if (coordinates.Values.Any(v => v == null))
            {
                // TODO: Log error.
                return;
            }

            var outputVariableValues = new Dictionary<string,List<TimeDependentOutputVariableValue>>();
            foreach (var name in doubleVariableNames)
            {
                outputVariableValues[name] = GetDefaultOutputVariableList(results, coordinates, double.NaN);
            }
            foreach (var name in boolVariableNames)
            {
                outputVariableValues[name] = GetDefaultOutputVariableList(results, coordinates, false);
            }
            outputVariableValues["Schadeontwikkeling"] = GetDefaultOutputVariableList(results, coordinates, double.NaN);

            foreach (var location in results)
            {
                var currentCoordinate = coordinates[location.Height]?.Coordinate;
                if (currentCoordinate == null)
                {
                    throw new Exception("Should not occur");
                }

                for (int i = 1; i < project.TimeSteps.Count; i++)
                {
                    if (doubleVariableNames.Any())
                    {
                        foreach (var variableName in doubleVariableNames.Where(v => location.PhysicsDouble.ContainsKey(v)))
                        {
                            outputVariableValues[variableName].Add(new TimeDependentOutputVariableValue(
                                currentCoordinate,
                                project.TimeSteps[i],
                                location.PhysicsDouble[variableName][i-1]));
                        }
                    }

                    if (boolVariableNames.Any())
                    {
                        foreach (var variableName in boolVariableNames.Where(v => location.PhysicsBool.ContainsKey(v)))
                        {
                            outputVariableValues[variableName].Add(new TimeDependentOutputVariableValue(
                                currentCoordinate,
                                project.TimeSteps[i],
                                location.PhysicsBool[variableName][i-1]));
                        }
                    }

                    if (location.DamageDevelopment != null)
                    {
                        outputVariableValues["Schadeontwikkeling"].Add(
                            new TimeDependentOutputVariableValue(
                                currentCoordinate, 
                                project.TimeSteps[i], 
                                location.DamageDevelopment[i-1]));
                    }
                }

                // TODO: Not time dependent
                /*OuterSlope
                RevetmentFailed
                RevetmentFailedAfter*/
            }

            var timeDependentOutputVariables = outputVariableValues.Select(v =>
                new TimeDependentOutputVariable(v.Key, v.Value.First().Value.GetType(), v.Value.ToArray()));
            project.TimeDependentOutputVariables.Clear();
            foreach (var variable in timeDependentOutputVariables)
            {
                project.TimeDependentOutputVariables.Add(variable);
            }

            project.OutputFileName = fileName;
            project.OnPropertyChanged(nameof(DikeErosionProject.OutputFileName));

        }

        private List<TimeDependentOutputVariableValue> GetDefaultOutputVariableList(OutputAtLocation[] results, Dictionary<double, OutputLocation?> coordinates, object value)
        {
            return results.Select(r =>
            {
                var coordinate = coordinates[r.Height]?.Coordinate;
                if (coordinate != null)
                {
                    return new TimeDependentOutputVariableValue(coordinate, project.TimeSteps[0], value);
                }

                throw new Exception("Should not occur");
            }).ToList();
        }

        private void ValidateFileName(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new DikeErosionException(DikeErosionExceptionType.FileNotFound);
            }
        }

        
    }
}
