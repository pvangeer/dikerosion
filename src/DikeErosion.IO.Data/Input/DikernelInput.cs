using DikeErosion.Data;

namespace DikeErosion.IO.Data.Input
{
    public class DikernelInput
    {
        public DikernelInput(IEnumerable<double> timeSteps, DikeProfile? profile, IEnumerable<OutputLocationSpecification> outputLocations)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            TimeSteps = timeSteps;
            OutputLocations = outputLocations;
            HydraulicConditions = new List<HydraulicConditionItem>();
        }

        public IEnumerable<double> TimeSteps { get; }

        public List<HydraulicConditionItem> HydraulicConditions { get; }

        public DikeProfile Profile { get; }

        public IEnumerable<OutputLocationSpecification> OutputLocations { get; }
    }
}