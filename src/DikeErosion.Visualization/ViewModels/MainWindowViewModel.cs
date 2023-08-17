using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private double currentTimeStep;
        private DikeErosionProject Project { get; }

        public MainWindowViewModel()
        {
            Project = new DikeErosionProject();
            Project.PropertyChanged += ProjectPropertyChanged;
            ContentViewModels = new ObservableCollection<ViewModelBase>
            {
                new CrossShoreChartViewModel(Project),
                new TimeLineViewModel(Project)
            };
        }

        public string InputFileName
        {
            get => Project.InputFileName;
            set
            {
                Project.InputFileName = value;
                Project.OnPropertyChanged(nameof(DikeErosionProject.InputFileName));
            }
        }

        public string OutputFileName
        {
            get => Project.OutputFileName;
            set
            {
                Project.OutputFileName = value;
                Project.OnPropertyChanged(nameof(DikeErosionProject.OutputFileName));
            }
        }

        public ICommand SelectInputFileCommand => new SelectInputFileCommand(Project);

        public ICommand SelectOutputFileCommand => new SelectOutputFileCommand(Project);

        public ObservableCollection<ViewModelBase> ContentViewModels { get; }

        private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DikeErosionProject.InputFileName):
                    OnPropertyChanged(nameof(InputFileName));
                    break;
                case nameof(DikeErosionProject.OutputFileName):
                    OnPropertyChanged(nameof(OutputFileName));
                    break;
            }
        }
    }
}
