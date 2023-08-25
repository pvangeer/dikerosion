using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Shapes;
using DikeErosion.Data;
using DikeErosion.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Path = System.IO.Path;

namespace DikeErosion.Visualization.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly CrossShoreChartViewModel crossShoreChartViewModel;
        private readonly TimeLineViewModel timeLineViewModel;

        private readonly DikeErosionProject project;

        public MainWindowViewModel()
        {
            project = new DikeErosionProject();

            crossShoreChartViewModel = new CrossShoreChartViewModel(project);
            timeLineViewModel = new TimeLineViewModel(project);

            SelectedContentViewModel = crossShoreChartViewModel;
        }

        public string WindowTitle => "Dijkerosie";

        public FileSelectionViewModel FileSelectionViewModel => new(project);

        public ViewState ViewState
        {
            get => project.ViewState;
            set
            {
                if (project.ViewState != value)
                {
                    project.ViewState = value;
                    SelectedContentViewModel = project.ViewState == ViewState.CrossShore ? crossShoreChartViewModel : timeLineViewModel;
                    OnPropertyChanged(nameof(SelectedContentViewModel));
                    OnPropertyChanged();
                }
            }
        }

        public ViewModelBase SelectedContentViewModel { get; private set; }

        public ICommand ReCalculateCommand => new RecalculateCommand(project);
    }

    public class RecalculateCommand : ICommand
    {
        private readonly DikeErosionProject project;

        public RecalculateCommand(DikeErosionProject project)
        {
            this.project = project;
            project.PropertyChanged += ProjectPropertyChanged;
        }

        private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DikeErosionProject.OutputFileName):
                    CanExecuteChanged?.Invoke(project, EventArgs.Empty);
                    break;
                case nameof(DikeErosionProject.OverwriteOutput):
                    CanExecuteChanged?.Invoke(project,EventArgs.Empty);
                    break;
            }
        }

        public bool CanExecute(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(project.OutputFileName) && 
                   Directory.Exists(Path.GetDirectoryName(project.OutputFileName)) && 
                   (project.OverwriteOutput && File.Exists(project.OutputFileName) || !File.Exists(project.OutputFileName));
        }

        public void Execute(object? parameter)
        {
            string? assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(assemblyFolder))
            {
                throw new Exception("Executable not found.");
            }
            var pathToDiKErnel = Path.Combine(assemblyFolder,"DiKErnel", "DiKErnel-cli.exe");

            var proc = Process.Start(pathToDiKErnel, new[]{"--invoerbestand", project.InputFileName, "--uitvoerbestand", project.OutputFileName, "--uitvoerniveau", "fysica"});
            proc.WaitForExit(4000); // TODO: which time-out is still ok? Make it input to?

            if (proc.ExitCode != 0)
            {
                // TODO: Log. Invalid run.
                return;
            }

            var importer = new DikeErosionOutputImporter(project);
            importer.Import(project.OutputFileName);
        }

        public event EventHandler? CanExecuteChanged;
    }
}
