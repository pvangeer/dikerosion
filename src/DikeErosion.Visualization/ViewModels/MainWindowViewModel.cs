using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly CrossShoreChartViewModel crossShoreChartViewModel;
        private readonly TimeLineViewModel timeLineViewModel;

        private DikeErosionProject Project { get; }

        public MainWindowViewModel()
        {
            Project = new DikeErosionProject();

            crossShoreChartViewModel = new CrossShoreChartViewModel(Project);
            timeLineViewModel = new TimeLineViewModel(Project);

            SelectedContentViewModel = crossShoreChartViewModel;
        }

        public string WindowTitle => "Dijkerosie";

        public FileSelectionViewModel FileSelectionViewModel => new(Project);

        public ViewState ViewState
        {
            get => Project.ViewState;
            set
            {
                if (Project.ViewState != value)
                {
                    Project.ViewState = value;
                    SelectedContentViewModel = Project.ViewState == ViewState.CrossShore ? crossShoreChartViewModel : timeLineViewModel;
                    OnPropertyChanged(nameof(SelectedContentViewModel));
                    OnPropertyChanged();
                }
            }
        }

        public ViewModelBase SelectedContentViewModel { get; private set; }
    }
}
