namespace DikeErosion.IO.Data.Input
{
    public class DikernelInput
    {
        public DikernelInput(IEnumerable<double> timeSteps, DikeProfile? profile, IEnumerable<OutputLocationSpecification> outputLocations)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            TimeSteps = timeSteps;
            Profile = profile;
            OutputLocations = outputLocations;
            CharacteristicPoints = new List<CharacteristicPoint>();
            HydraulicConditions = new List<HydraulicCondition>();
        }

        public IEnumerable<double> TimeSteps { get; }

        public List<HydraulicCondition> HydraulicConditions { get; }

        public DikeProfile Profile { get; }

        public IEnumerable<CharacteristicPoint> CharacteristicPoints { get; }

        public IEnumerable<OutputLocationSpecification> OutputLocations { get; }
    }
}