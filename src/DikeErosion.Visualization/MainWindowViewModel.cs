using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DikeErosion.Data;

namespace DikeErosion.Visualization
{
    public class MainWindowViewModel : ViewModelBase
    {
        private double currentTimeStep;
        private DikeErosionProject Project { get; }

        public MainWindowViewModel()
        {
            Project = new DikeErosionProject();
            Project.PropertyChanged += ProjectPropertyChanged;
            Project.TimeSteps.CollectionChanged += TimeStepsCollectionChanged;
            ChartAreaViewModel = new CrossShoreChartViewModel(Project);
        }

        private void TimeStepsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            TimeSteps = Project.TimeSteps.ToList();
            CurrentTimeStep = TimeSteps.Any() ? TimeSteps.Min() : 0.0;
            OnPropertyChanged(nameof(CurrentTimeStep));
            OnPropertyChanged(nameof(TimeSteps));
        }

        public CrossShoreChartViewModel ChartAreaViewModel { get; }

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

        public List<double>? TimeSteps { get; set; }

        public double CurrentTimeStep
        {
            get => currentTimeStep;
            set
            {
                currentTimeStep = value;
                ChartAreaViewModel.CurrentTimeStep = currentTimeStep;
            }
        }

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
