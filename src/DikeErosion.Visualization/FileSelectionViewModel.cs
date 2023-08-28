using System.ComponentModel;
using System.Windows.Input;
using DikeErosion.Data;
using DikeErosion.Visualization.ViewModels;

namespace DikeErosion.Visualization;

public class FileSelectionViewModel : ViewModelBase
{
    private readonly DikeErosionProject project;

    public FileSelectionViewModel(DikeErosionProject project)
    {
        this.project = project;
        this.project.PropertyChanged += ProjectPropertyChanged;
    }

    public ICommand SelectInputFileCommand => new SelectInputFileCommand(project);

    public ICommand SelectOutputFileCommand => new SelectOutputFileCommand(project);

    public string InputFileName
    {
        get => project.InputFileName;
        set
        {
            project.InputFileName = value;
            project.OnPropertyChanged();
        }
    }

    public string OutputFileName
    {
        get => project.OutputFileName;
        set
        {
            project.OutputFileName = value;
            project.OnPropertyChanged();
        }
    }

    public bool OverwriteOutput
    {
        get => project.OverwriteOutput;
        set
        {
            project.OverwriteOutput = value;
            project.OnPropertyChanged();
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