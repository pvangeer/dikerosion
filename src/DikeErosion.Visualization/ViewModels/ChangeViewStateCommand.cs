using System;
using System.Windows.Input;
using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels;

public class ChangeViewStateCommand : ICommand
{
    private readonly DikeErosionProject project;
    private readonly ViewState newState;

    public ChangeViewStateCommand(DikeErosionProject project, ViewState newState)
    {
        this.project = project;
        this.newState = newState;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        project.ViewState = newState;
        project.OnPropertyChanged(nameof(DikeErosionProject.ViewState));
    }

    public event EventHandler? CanExecuteChanged;
}