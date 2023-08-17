using System.Collections.ObjectModel;
using DikeErosion.Data.CrossShoreProfile;

namespace DikeErosion.Data
{
    public class DikeErosionProject : NotifyPropertyChangedObservable
    {
        public DikeErosionProject()
        {
            InputFileName = "";
            OutputFileName = "";

            TimeSteps = new ObservableCollection<double>();
            Profile = new Profile();
            OutputLocations = new ObservableCollection<OutputLocation>();
            HydraulicConditions = new ObservableCollection<HydraulicCondition>();
        }

        public string InputFileName { get; set; }

        public string OutputFileName { get; set; }

        public Profile Profile { get; }

        public ObservableCollection<double> TimeSteps { get; }

        public ObservableCollection<OutputLocation> OutputLocations { get; }

        public ObservableCollection<HydraulicCondition> HydraulicConditions { get; }
    }
}