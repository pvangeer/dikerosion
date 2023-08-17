using DikeErosion.Data;
using DikeErosion.Data.CrossShoreProfile;
using DikeErosion.Data.ExceptionHandling;

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
            var firstResult = results.FirstOrDefault();
            if (firstResult == null)
            {
                return;
            }

            var doublesPhysicsResultsAvailable = firstResult.PhysicsDouble.Any();
            var boolPhysicsResultsAvailable = firstResult.PhysicsBool.Any();
            if ((doublesPhysicsResultsAvailable && firstResult.PhysicsDouble.Values.First().Length != project.TimeSteps.Count -1) ||
                (boolPhysicsResultsAvailable && firstResult.PhysicsDouble.Values.First().Length != project.TimeSteps.Count - 1))
            {
                return;
            }

            var outputVariableValues = new Dictionary<string,List<TimeDependentOutputVariableValue>>();
            var doubleVariableNames = firstResult.PhysicsDouble.Keys.ToArray();
            var boolVariableNames = firstResult.PhysicsBool.Keys.ToArray();
            foreach (var name in doubleVariableNames)
            {
                outputVariableValues[name] = new List<TimeDependentOutputVariableValue>();
            }
            foreach (var name in boolVariableNames)
            {
                outputVariableValues[name] = new List<TimeDependentOutputVariableValue>();
            }
            outputVariableValues["Schadeontwikkeling"] = new List<TimeDependentOutputVariableValue>();

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

            foreach (var location in results)
            {
                var currentCoordinate = coordinates[location.Height].Coordinate;
                for (int i = 0; i < project.TimeSteps.Count - 1; i++)
                {
                    if (doublesPhysicsResultsAvailable)
                    {
                        foreach (var variableName in doubleVariableNames)
                        {
                            outputVariableValues[variableName].Add(new TimeDependentOutputVariableValue(
                                currentCoordinate,
                                project.TimeSteps[i + 1],
                                location.PhysicsDouble[variableName][i]));
                        }
                    }

                    if (boolPhysicsResultsAvailable)
                    {
                        foreach (var variableName in boolVariableNames)
                        {
                            outputVariableValues[variableName].Add(new TimeDependentOutputVariableValue(
                                currentCoordinate,
                                project.TimeSteps[i + 1],
                                location.PhysicsBool[variableName][i]));
                        }
                    }

                    if (location.DamageDevelopment != null)
                    {
                        outputVariableValues["Schadeontwikkeling"].Add(new TimeDependentOutputVariableValue(currentCoordinate, project.TimeSteps[i], location.DamageDevelopment[i]));
                    }
                }

                // TODO: Not time dependent
                /*OuterSlope
                    RevetmentFailed
                RevetmentFailedAfter*/

            }

            var timeDependentOutputVariables = outputVariableValues.Select(v =>
                new TimeDependentOutputVariable(v.Key, v.Value.First().GetType(), v.Value.ToArray()));
            project.TimeDependentOutputVariables.Clear();
            foreach (var variable in timeDependentOutputVariables)
            {
                project.TimeDependentOutputVariables.Add(variable);
            }

            project.OutputFileName = fileName;

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
