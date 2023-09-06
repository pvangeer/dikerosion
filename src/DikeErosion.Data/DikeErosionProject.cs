using System.Collections.ObjectModel;
using DikeErosion.Data.CrossShoreProfile;
using DikeErosion.Visualization.ViewModels;

namespace DikeErosion.Data;

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

        TimeDependentOutputVariables = new ObservableCollection<TimeDependentOutputVariable>();

        LocationSpecificOutputVariables = new ObservableCollection<LocationSpecificOutput>();
    }

    public ViewState ViewState { get; set; }

    public string InputFileName { get; set; }

    public string OutputFileName { get; set; }

    #region DikeErosionCalculation

    public Profile Profile { get; }

    public ObservableCollection<double> TimeSteps { get; }

    public ObservableCollection<OutputLocation> OutputLocations { get; }

    public ObservableCollection<HydraulicCondition> HydraulicConditions { get; }

    public ObservableCollection<TimeDependentOutputVariable> TimeDependentOutputVariables { get; }

    public ObservableCollection<LocationSpecificOutput> LocationSpecificOutputVariables { get; }

    public bool OverwriteOutput { get; set; }

    #endregion
}