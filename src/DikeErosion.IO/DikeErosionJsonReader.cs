using DikeErosion.IO.Data.Input;
using DikeErosion.IO.Data.Output;
using Newtonsoft.Json.Linq;

namespace DikeErosion.IO
{
    public static class DikeErosionJsonReader
    {
        private const string OutputDataObjectPropertyName = "uitvoerdata";
        private const string LocationsObjectPropertyName = "locaties";
        private const string FailureObjectPropertyName = "falen";
        private const string FailureOutputPropertyName = "faalgebeurtenis";
        private const string FailureTimeOutputPropertyName = "faaltijd";
        private const string DamageOutputPropertyName = "schade";
        private const string DamagePerTimeStepOutputPropertyName = "schadegetalPerTijdstap";
        private const string PhysicsObjectPropertyName = "fysica";
        private const string HeightOutputPropertyName = "hoogteLocatie";
        private const string SlopeOutputPropertyName = "hellingBuitentalud";

        //private const string InputDataObjectPropertyName = "rekendata";
        private const string TimeStepsPropertyName = "tijdstippen";
        private const string HydraulicConditionsInputPropertyName = "hydraulischeBelastingen";
        private const string ProfileDefinitionPropertyName = "dijkprofiel";
        private const string ProfileXPositionsPropertyName = "posities";
        private const string ProfileZPositionsPropertyName = "hoogten";
        private const string ToeOuterSlopePropertyName = "teenBuitenzijde";
        private const string BermCrownOuterSlopePropertyName = "kruinBermBuitenzijde";
        private const string InsetBermOuterSlopePropertyName = "insteekBermBuitenzijde";
        private const string CrownOuterSlopePropertyName = "kruinBuitenzijde";

        private const string OutputLocationsPropertyName = "locaties";
        private const string LocationPositionPropertyName = "positie";
        private const string BeginDamagePropertyName = "beginschade";
        private const string CalculationMethodPropertyName = "rekenmethode";
        private const string TopLayerTypePropertyName = "typeToplaag";

        public static DikernelInput ReadInput(string fileName)
        {
            var jInput = ReadJasonFileContent(fileName);

            if (jInput.Count < 1)
                throw new Exception("Unknown file format");

            var timeSteps = ReadAsArrayOfDoubles(jInput, TimeStepsPropertyName);

            var hydraulicConditions = new Dictionary<string, double[]>();
            var jHydraulicConditions = ReadAsJObject(jInput, HydraulicConditionsInputPropertyName);
            if (jHydraulicConditions != null)
            {
                foreach (var jHydraulicCondition in jHydraulicConditions)
                {
                    var name = jHydraulicCondition.Key;
                    if (jHydraulicCondition.Value?.First?.Type == JTokenType.Float)
                    {
                        hydraulicConditions[name] = jHydraulicCondition.Value.Select(v => v.ToDouble()).ToArray();
                    }
                    if (jHydraulicCondition.Value?.First?.Type == JTokenType.Integer)
                    {
                        hydraulicConditions[name] = jHydraulicCondition.Value.Select(v => v.ToDouble()).ToArray();
                    }
                }
            }

            DikeProfile? dikeProfile = null;
            var jProfileDefinition = ReadAsJObject(jInput, ProfileDefinitionPropertyName);
            if (jProfileDefinition != null)
            {
                var coordinates = new List<ProfileCoordinate>();

                var xPositions = ReadAsArrayOfDoubles(jProfileDefinition, ProfileXPositionsPropertyName);
                var zPositions = ReadAsArrayOfDoubles(jProfileDefinition, ProfileZPositionsPropertyName);
                if (xPositions.Length == zPositions.Length)
                {
                    for (int i = 0; i < xPositions.Length; i++)
                    {
                        coordinates.Add(new ProfileCoordinate(xPositions[i], zPositions[i]));
                    }
                }

                var toeOuterSlope = ToCoordinate(ReadAsDouble(jProfileDefinition, ToeOuterSlopePropertyName), coordinates);
                var bermCrownOuterSlope = ToCoordinate(ReadAsDouble(jProfileDefinition, BermCrownOuterSlopePropertyName), coordinates);
                var insetBermOuterSlope = ToCoordinate(ReadAsDouble(jProfileDefinition, InsetBermOuterSlopePropertyName), coordinates);
                var crownOuterSlope = ToCoordinate(ReadAsDouble(jProfileDefinition, CrownOuterSlopePropertyName), coordinates);

                dikeProfile = new DikeProfile(coordinates, toeOuterSlope, bermCrownOuterSlope, insetBermOuterSlope, crownOuterSlope);
            }

            var outputLocations = new List<OutputLocationSpecification>();
            var jOutputLocations = ReadAsJArray(jInput, OutputLocationsPropertyName);
            if (jOutputLocations != null)
            {
                foreach (var jOutputLocation in jOutputLocations.OfType<JObject>())
                {
                    outputLocations.Add(
                        new OutputLocationSpecification(
                            ReadAsDouble(jOutputLocation, LocationPositionPropertyName),
                            ApplyDefaultValueIfNaN(ReadAsDouble(jOutputLocation, BeginDamagePropertyName),0.0),
                            ReadAsString(jOutputLocation, CalculationMethodPropertyName),
                            ReadAsString(jOutputLocation, TopLayerTypePropertyName)
                        ));
                }
            }

            // TODO: Read calculation settings
            var dikernelInput = new DikernelInput(timeSteps, dikeProfile, outputLocations);
            dikernelInput.HydraulicConditions.AddRange(hydraulicConditions.Select(c => new HydraulicCondition(c.Key, c.Value)));
            return dikernelInput;
        }

        public static OutputLocation[] ReadResults(string fileName)
        {
            var json = ReadJasonFileContent(fileName);

            var jOutput = ReadAsJObject(json, OutputDataObjectPropertyName) ?? throw new Exception("Could not read output.");

            var jOutputLocations = ReadAsJArray(jOutput, LocationsObjectPropertyName);
            if (jOutputLocations == null || jOutputLocations.Count == 0 || jOutputLocations.First?.Type != JTokenType.Object)
            {
                return Array.Empty<OutputLocation>();
            }

            var locations = new List<OutputLocation>();
            foreach (var jOutputLocation in jOutputLocations.OfType<JObject>())
            {
                var revetmentFailed = false;
                var revetmentFailedAfter = double.NaN;
                var damage = Array.Empty<double>();
                var height = double.NaN;
                var outerSlope = double.NaN;

                var jFailureOutputObject = ReadAsJObject(jOutputLocation, FailureObjectPropertyName);
                if (jFailureOutputObject != null)
                {
                    revetmentFailed = ReadAsBool(jFailureOutputObject, FailureOutputPropertyName);
                    revetmentFailedAfter = ReadAsDouble(jFailureOutputObject, FailureTimeOutputPropertyName);
                }

                var jDamageOutputObject = ReadAsJObject(jOutputLocation, DamageOutputPropertyName);
                if (jDamageOutputObject != null)
                {
                    damage = ReadAsArrayOfDoubles(jDamageOutputObject, DamagePerTimeStepOutputPropertyName);
                }

                var physicsDoublesDictionary = new Dictionary<string, double[]>();
                var physicsBoolDictionary = new Dictionary<string, bool[]>();
                var jPhysics = ReadAsJObject(jOutputLocation, PhysicsObjectPropertyName);
                if (jPhysics != null)
                {
                    foreach (var jPhysicsOutput in jPhysics)
                    {
                        var name = jPhysicsOutput.Key;
                        switch (name)
                        {
                            case HeightOutputPropertyName:
                                height = jPhysicsOutput.Value.ToDouble();
                                break;
                            case SlopeOutputPropertyName:
                                outerSlope = jPhysicsOutput.Value.ToDouble(); 
                                break;
                            default:
                                if (jPhysicsOutput.Value?.First?.Type == JTokenType.Float)
                                {
                                    physicsDoublesDictionary[name] = jPhysicsOutput.Value.Select(v => v.ToDouble()).ToArray();
                                }
                                if (jPhysicsOutput.Value?.First?.Type == JTokenType.Boolean)
                                {
                                    physicsBoolDictionary[name] = jPhysicsOutput.Value.Select(v => v.ToBool()).ToArray();
                                }
                                break;
                        }
                    }
                }

                var location = new OutputLocation(height, outerSlope, revetmentFailed, revetmentFailedAfter, damage);
                foreach (var item in physicsDoublesDictionary)
                {
                    location.PhysicsDouble[item.Key] = item.Value;
                }
                foreach (var item in physicsBoolDictionary)
                {
                    location.PhysicsBool[item.Key] = item.Value;
                }
                locations.Add(location);
            }

            return locations.ToArray();
        }

        private static double ApplyDefaultValueIfNaN(double value, double defaultValue)
        {
            return double.IsNaN(value) ? defaultValue : value;
        }

        private static ProfileCoordinate? ToCoordinate(double xPosition, List<ProfileCoordinate> coordinates)
        {
            return coordinates.FirstOrDefault(c => Math.Abs(c.X - xPosition) < 1E-8);
        }

        private static JObject ReadJasonFileContent(string fileName)
        {
            // TODO: Validate file and format.
            return JObject.Parse(File.ReadAllText(fileName));
        }

        private static double[] ReadAsArrayOfDoubles(JObject jObject, string propertyName)
        {
            var jArray = ReadAsJArray(jObject, propertyName);
            return jArray == null ? Array.Empty<double>() : jArray.Select(t => t.ToDouble()).ToArray();
        }

        private static double ReadAsDouble(JObject jObject, string propertyName)
        {
            return ReadAsJToken(jObject, propertyName).ToDouble();
        }

        private static bool ReadAsBool(JObject jObject, string propertyName)
        {
            var jToken = ReadAsJToken(jObject, propertyName);
            return jToken != null && (bool)jToken;
        }

        private static string ReadAsString(JObject jObject, string propertyName)
        {
            var jToken = ReadAsJToken(jObject, propertyName);
            return jToken == null ? string.Empty : jToken.ToString();
        }

        private static JArray? ReadAsJArray(JObject output, string propertyName)
        {
            return ReadAsJToken(output, propertyName) as JArray;
        }

        private static JObject? ReadAsJObject(JObject outputJson, string propertyName)
        {
            return ReadAsJToken(outputJson, propertyName) as JObject;
        }

        private static JToken? ReadAsJToken(JObject output, string propertyName)
        {
            return output[propertyName];
        }
    }
}