using System.ComponentModel;
using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DikeErosionProject project;
        private readonly TimeLineViewModel timeLineViewModel;
        private readonly CrossShoreChartViewModel crossShoreChartViewModel;

        public MainWindowViewModel()
        {
            project = new DikeErosionProject();
            project.PropertyChanged += ProjectPropertyChanged;

            RibbonViewModel = new RibbonViewModel(project);
            crossShoreChartViewModel = new CrossShoreChartViewModel(project);
            timeLineViewModel = new TimeLineViewModel(project);
        }

        public string WindowTitle => "Dijkerosie";

        public RibbonViewModel RibbonViewModel { get; }

        public ViewModelBase SelectedContentViewModel => project.ViewState == ViewState.CrossShore
            ? crossShoreChartViewModel
            : timeLineViewModel;

        private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DikeErosionProject.ViewState):
                    OnPropertyChanged(nameof(SelectedContentViewModel));
                    break;
            }
        }
    }
}
