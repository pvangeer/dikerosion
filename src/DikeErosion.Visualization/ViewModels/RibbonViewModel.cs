using System.Windows.Input;
using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels
{
    public class RibbonViewModel : ViewModelBase
    {
        private readonly DikeErosionProject project;

        public RibbonViewModel() : this(new DikeErosionProject()) { }

        public RibbonViewModel(DikeErosionProject project)
        {
            this.project = project;
        }

        public FileSelectionViewModel FileSelectionViewModel => new(project);

        public ViewState ViewState
        {
            get => project.ViewState;
            set
            {
                if (project.ViewState != value)
                {
                    project.ViewState = value;
                    project.OnPropertyChanged();
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ReCalculateCommand => new RecalculateCommand(project);
    }
}
