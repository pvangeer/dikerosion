using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels;

public class TimeLineViewModel : ViewModelBase
{
    private readonly DikeErosionProject project;

    public TimeLineViewModel(DikeErosionProject project)
    {
        this.project = project;
    }

    public override string Title => "Tijdlijn";
}